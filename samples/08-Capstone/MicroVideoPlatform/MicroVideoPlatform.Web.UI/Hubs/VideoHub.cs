using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MicroVideoPlatform.Web.UI.Hubs;

/// <summary>
/// SignalR hub for real-time video platform features.
/// Handles comments, likes, online users, and notifications.
/// </summary>
public class VideoHub : Hub
{
    // Track users connected to each video room
    private static readonly ConcurrentDictionary<string, HashSet<string>> _videoRooms = new();

    // Track user information (connectionId -> userId)
    private static readonly ConcurrentDictionary<string, string> _connectedUsers = new();

    // Track online users per video (videoId -> List<userId>)
    private static readonly ConcurrentDictionary<string, HashSet<string>> _videoViewers = new();

    private readonly ILogger<VideoHub> _logger;

    public VideoHub(ILogger<VideoHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("Client connected: {ConnectionId}", connectionId);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        _logger.LogInformation("Client disconnected: {ConnectionId}", connectionId);

        // Remove user from all rooms and tracking
        if (_connectedUsers.TryRemove(connectionId, out var userId))
        {
            // Find all videos this user was watching
            foreach (var (videoId, viewers) in _videoViewers)
            {
                if (viewers.Remove(userId))
                {
                    await Clients.Group($"video_{videoId}").SendAsync("UserLeft", new
                    {
                        UserId = userId,
                        OnlineCount = viewers.Count,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            // Remove from video rooms
            foreach (var (videoId, connections) in _videoRooms)
            {
                connections.Remove(connectionId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Registers a user with their ID.
    /// </summary>
    public async Task RegisterUser(string userId)
    {
        _connectedUsers[Context.ConnectionId] = userId;
        _logger.LogInformation("User registered: {UserId} with connection {ConnectionId}", userId, Context.ConnectionId);

        await Clients.Caller.SendAsync("UserRegistered", new
        {
            Success = true,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Joins a video room to receive real-time updates for that video.
    /// </summary>
    public async Task JoinVideoRoom(string videoId, string userId)
    {
        var connectionId = Context.ConnectionId;

        // Add to SignalR group
        await Groups.AddToGroupAsync(connectionId, $"video_{videoId}");

        // Track connection in video room
        _videoRooms.AddOrUpdate(
            videoId,
            _ => new HashSet<string> { connectionId },
            (_, connections) => { connections.Add(connectionId); return connections; }
        );

        // Track user as viewer
        _videoViewers.AddOrUpdate(
            videoId,
            _ => new HashSet<string> { userId },
            (_, viewers) => { viewers.Add(userId); return viewers; }
        );

        var onlineCount = _videoViewers[videoId].Count;

        _logger.LogInformation("User {UserId} joined video room {VideoId}. Online: {Count}", userId, videoId, onlineCount);

        // Notify all users in the room
        await Clients.Group($"video_{videoId}").SendAsync("UserJoined", new
        {
            UserId = userId,
            VideoId = videoId,
            OnlineCount = onlineCount,
            Timestamp = DateTime.UtcNow
        });

        // Send current online users to the caller
        await Clients.Caller.SendAsync("OnlineUsers", new
        {
            VideoId = videoId,
            Users = _videoViewers[videoId].ToList(),
            Count = onlineCount
        });
    }

    /// <summary>
    /// Leaves a video room.
    /// </summary>
    public async Task LeaveVideoRoom(string videoId, string userId)
    {
        var connectionId = Context.ConnectionId;

        // Remove from SignalR group
        await Groups.RemoveFromGroupAsync(connectionId, $"video_{videoId}");

        // Remove from tracking
        if (_videoRooms.TryGetValue(videoId, out var connections))
        {
            connections.Remove(connectionId);
        }

        if (_videoViewers.TryGetValue(videoId, out var viewers))
        {
            viewers.Remove(userId);
            var onlineCount = viewers.Count;

            _logger.LogInformation("User {UserId} left video room {VideoId}. Online: {Count}", userId, videoId, onlineCount);

            // Notify all users in the room
            await Clients.Group($"video_{videoId}").SendAsync("UserLeft", new
            {
                UserId = userId,
                VideoId = videoId,
                OnlineCount = onlineCount,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Broadcasts a new comment to all users watching the video.
    /// </summary>
    public async Task SendComment(string videoId, string userId, string userName, string commentText)
    {
        var comment = new
        {
            CommentId = Guid.NewGuid().ToString(),
            VideoId = videoId,
            UserId = userId,
            UserName = userName,
            Text = commentText,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Broadcasting comment from {UserName} on video {VideoId}", userName, videoId);

        await Clients.Group($"video_{videoId}").SendAsync("NewComment", comment);
    }

    /// <summary>
    /// Broadcasts a comment deletion to all users watching the video.
    /// </summary>
    public async Task DeleteComment(string videoId, string commentId, string userId)
    {
        var deleteEvent = new
        {
            CommentId = commentId,
            VideoId = videoId,
            DeletedBy = userId,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Broadcasting comment deletion {CommentId} on video {VideoId}", commentId, videoId);

        await Clients.Group($"video_{videoId}").SendAsync("CommentDeleted", deleteEvent);
    }

    /// <summary>
    /// Broadcasts like count update to all users watching the video.
    /// </summary>
    public async Task UpdateLikeCount(string videoId, int likesCount, int dislikesCount)
    {
        var update = new
        {
            VideoId = videoId,
            LikesCount = likesCount,
            DislikesCount = dislikesCount,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Broadcasting like count update for video {VideoId}: {Likes} likes, {Dislikes} dislikes",
            videoId, likesCount, dislikesCount);

        await Clients.Group($"video_{videoId}").SendAsync("LikeCountUpdated", update);
    }

    /// <summary>
    /// Broadcasts view count update to all users watching the video.
    /// </summary>
    public async Task UpdateViewCount(string videoId, int viewCount)
    {
        var update = new
        {
            VideoId = videoId,
            ViewCount = viewCount,
            Timestamp = DateTime.UtcNow
        };

        await Clients.Group($"video_{videoId}").SendAsync("ViewCountUpdated", update);
    }

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    public async Task SendNotificationToUser(string targetUserId, string title, string message, string type = "info")
    {
        var notification = new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Title = title,
            Message = message,
            Type = type, // "info", "success", "warning", "error"
            Timestamp = DateTime.UtcNow
        };

        // Find all connections for this user
        var userConnections = _connectedUsers
            .Where(kvp => kvp.Value == targetUserId)
            .Select(kvp => kvp.Key)
            .ToList();

        if (userConnections.Any())
        {
            await Clients.Clients(userConnections).SendAsync("ReceiveNotification", notification);
            _logger.LogInformation("Sent notification to user {UserId}: {Title}", targetUserId, title);
        }
    }

    /// <summary>
    /// Broadcasts a notification to all connected users.
    /// </summary>
    public async Task BroadcastNotification(string title, string message, string type = "info")
    {
        var notification = new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        };

        await Clients.All.SendAsync("ReceiveNotification", notification);
        _logger.LogInformation("Broadcast notification to all users: {Title}", title);
    }

    /// <summary>
    /// Notifies users when a new video is uploaded.
    /// </summary>
    public async Task NotifyNewVideoUpload(string videoId, string title, string uploaderName, string category)
    {
        var notification = new
        {
            NotificationId = Guid.NewGuid().ToString(),
            Type = "new_video",
            VideoId = videoId,
            Title = title,
            UploaderName = uploaderName,
            Category = category,
            Message = $"New video uploaded: {title}",
            Timestamp = DateTime.UtcNow
        };

        await Clients.All.SendAsync("NewVideoUploaded", notification);
        _logger.LogInformation("Notified new video upload: {Title} by {Uploader}", title, uploaderName);
    }

    /// <summary>
    /// Gets the current online user count for a video.
    /// </summary>
    public async Task<int> GetOnlineCount(string videoId)
    {
        if (_videoViewers.TryGetValue(videoId, out var viewers))
        {
            return viewers.Count;
        }
        return 0;
    }

    /// <summary>
    /// Gets statistics about the hub.
    /// </summary>
    public async Task<object> GetHubStats()
    {
        var stats = new
        {
            TotalConnections = _connectedUsers.Count,
            ActiveVideoRooms = _videoRooms.Count,
            TotalViewers = _videoViewers.Sum(kvp => kvp.Value.Count),
            Timestamp = DateTime.UtcNow
        };

        return stats;
    }
}
