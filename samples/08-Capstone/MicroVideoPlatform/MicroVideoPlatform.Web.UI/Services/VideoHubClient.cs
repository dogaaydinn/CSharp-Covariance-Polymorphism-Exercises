using Microsoft.AspNetCore.SignalR.Client;

namespace MicroVideoPlatform.Web.UI.Services;

/// <summary>
/// Client service for SignalR VideoHub communication.
/// Provides methods to interact with real-time features.
/// </summary>
public class VideoHubClient : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<VideoHubClient> _logger;
    private readonly string _hubUrl;

    // Event handlers for receiving hub messages
    public event Action<CommentReceivedEventArgs>? CommentReceived;
    public event Action<CommentDeletedEventArgs>? CommentDeleted;
    public event Action<LikeCountUpdatedEventArgs>? LikeCountUpdated;
    public event Action<ViewCountUpdatedEventArgs>? ViewCountUpdated;
    public event Action<UserJoinedEventArgs>? UserJoined;
    public event Action<UserLeftEventArgs>? UserLeft;
    public event Action<OnlineUsersEventArgs>? OnlineUsersReceived;
    public event Action<NotificationEventArgs>? NotificationReceived;
    public event Action<NewVideoEventArgs>? NewVideoUploaded;
    public event Action<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    public VideoHubClient(ILogger<VideoHubClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _hubUrl = configuration["SignalR:HubUrl"] ?? "/videohub";
    }

    /// <summary>
    /// Gets the current connection state.
    /// </summary>
    public HubConnectionState State => _hubConnection?.State ?? HubConnectionState.Disconnected;

    /// <summary>
    /// Gets whether the client is connected.
    /// </summary>
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Initializes and starts the SignalR connection.
    /// </summary>
    public async Task StartAsync()
    {
        if (_hubConnection != null)
        {
            _logger.LogWarning("Hub connection already initialized");
            return;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
            .Build();

        // Register event handlers
        RegisterHandlers();

        // Handle reconnection events
        _hubConnection.Reconnecting += OnReconnecting;
        _hubConnection.Reconnected += OnReconnected;
        _hubConnection.Closed += OnClosed;

        try
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("SignalR connection started successfully");
            NotifyConnectionStateChanged(HubConnectionState.Connected);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting SignalR connection");
            throw;
        }
    }

    /// <summary>
    /// Stops the SignalR connection.
    /// </summary>
    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            _logger.LogInformation("SignalR connection stopped");
            NotifyConnectionStateChanged(HubConnectionState.Disconnected);
        }
    }

    /// <summary>
    /// Registers a user with their ID.
    /// </summary>
    public async Task RegisterUserAsync(string userId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("RegisterUser", userId);
        _logger.LogInformation("User registered: {UserId}", userId);
    }

    /// <summary>
    /// Joins a video room to receive real-time updates.
    /// </summary>
    public async Task JoinVideoRoomAsync(string videoId, string userId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("JoinVideoRoom", videoId, userId);
        _logger.LogInformation("Joined video room: {VideoId}", videoId);
    }

    /// <summary>
    /// Leaves a video room.
    /// </summary>
    public async Task LeaveVideoRoomAsync(string videoId, string userId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("LeaveVideoRoom", videoId, userId);
        _logger.LogInformation("Left video room: {VideoId}", videoId);
    }

    /// <summary>
    /// Sends a comment to the video room.
    /// </summary>
    public async Task SendCommentAsync(string videoId, string userId, string userName, string commentText)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("SendComment", videoId, userId, userName, commentText);
        _logger.LogInformation("Comment sent to video {VideoId}", videoId);
    }

    /// <summary>
    /// Deletes a comment from the video room.
    /// </summary>
    public async Task DeleteCommentAsync(string videoId, string commentId, string userId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("DeleteComment", videoId, commentId, userId);
    }

    /// <summary>
    /// Updates like count for a video.
    /// </summary>
    public async Task UpdateLikeCountAsync(string videoId, int likesCount, int dislikesCount)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("UpdateLikeCount", videoId, likesCount, dislikesCount);
    }

    /// <summary>
    /// Updates view count for a video.
    /// </summary>
    public async Task UpdateViewCountAsync(string videoId, int viewCount)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("UpdateViewCount", videoId, viewCount);
    }

    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    public async Task SendNotificationToUserAsync(string targetUserId, string title, string message, string type = "info")
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        await _hubConnection.InvokeAsync("SendNotificationToUser", targetUserId, title, message, type);
    }

    /// <summary>
    /// Gets the current online user count for a video.
    /// </summary>
    public async Task<int> GetOnlineCountAsync(string videoId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            throw new InvalidOperationException("Hub connection not established");
        }

        return await _hubConnection.InvokeAsync<int>("GetOnlineCount", videoId);
    }

    private void RegisterHandlers()
    {
        if (_hubConnection == null) return;

        _hubConnection.On<object>("NewComment", (comment) =>
        {
            var props = comment.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(comment));
            CommentReceived?.Invoke(new CommentReceivedEventArgs
            {
                CommentId = props["CommentId"]?.ToString() ?? string.Empty,
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                UserId = props["UserId"]?.ToString() ?? string.Empty,
                UserName = props["UserName"]?.ToString() ?? string.Empty,
                Text = props["Text"]?.ToString() ?? string.Empty,
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("CommentDeleted", (deleteEvent) =>
        {
            var props = deleteEvent.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(deleteEvent));
            CommentDeleted?.Invoke(new CommentDeletedEventArgs
            {
                CommentId = props["CommentId"]?.ToString() ?? string.Empty,
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                DeletedBy = props["DeletedBy"]?.ToString() ?? string.Empty,
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("LikeCountUpdated", (update) =>
        {
            var props = update.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(update));
            LikeCountUpdated?.Invoke(new LikeCountUpdatedEventArgs
            {
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                LikesCount = Convert.ToInt32(props["LikesCount"]),
                DislikesCount = Convert.ToInt32(props["DislikesCount"]),
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("ViewCountUpdated", (update) =>
        {
            var props = update.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(update));
            ViewCountUpdated?.Invoke(new ViewCountUpdatedEventArgs
            {
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                ViewCount = Convert.ToInt32(props["ViewCount"]),
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("UserJoined", (joinEvent) =>
        {
            var props = joinEvent.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(joinEvent));
            UserJoined?.Invoke(new UserJoinedEventArgs
            {
                UserId = props["UserId"]?.ToString() ?? string.Empty,
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                OnlineCount = Convert.ToInt32(props["OnlineCount"]),
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("UserLeft", (leaveEvent) =>
        {
            var props = leaveEvent.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(leaveEvent));
            UserLeft?.Invoke(new UserLeftEventArgs
            {
                UserId = props["UserId"]?.ToString() ?? string.Empty,
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                OnlineCount = Convert.ToInt32(props["OnlineCount"]),
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("OnlineUsers", (usersEvent) =>
        {
            var props = usersEvent.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(usersEvent));
            OnlineUsersReceived?.Invoke(new OnlineUsersEventArgs
            {
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                Users = (props["Users"] as IEnumerable<object>)?.Select(u => u.ToString() ?? string.Empty).ToList() ?? new List<string>(),
                Count = Convert.ToInt32(props["Count"])
            });
        });

        _hubConnection.On<object>("ReceiveNotification", (notification) =>
        {
            var props = notification.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(notification));
            NotificationReceived?.Invoke(new NotificationEventArgs
            {
                NotificationId = props["NotificationId"]?.ToString() ?? string.Empty,
                Title = props["Title"]?.ToString() ?? string.Empty,
                Message = props["Message"]?.ToString() ?? string.Empty,
                Type = props["Type"]?.ToString() ?? "info",
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });

        _hubConnection.On<object>("NewVideoUploaded", (videoEvent) =>
        {
            var props = videoEvent.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(videoEvent));
            NewVideoUploaded?.Invoke(new NewVideoEventArgs
            {
                VideoId = props["VideoId"]?.ToString() ?? string.Empty,
                Title = props["Title"]?.ToString() ?? string.Empty,
                UploaderName = props["UploaderName"]?.ToString() ?? string.Empty,
                Category = props["Category"]?.ToString() ?? string.Empty,
                Message = props["Message"]?.ToString() ?? string.Empty,
                Timestamp = props["Timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            });
        });
    }

    private Task OnReconnecting(Exception? exception)
    {
        _logger.LogWarning(exception, "SignalR connection lost, reconnecting...");
        NotifyConnectionStateChanged(HubConnectionState.Reconnecting);
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation("SignalR reconnected with connection ID: {ConnectionId}", connectionId);
        NotifyConnectionStateChanged(HubConnectionState.Connected);
        return Task.CompletedTask;
    }

    private Task OnClosed(Exception? exception)
    {
        _logger.LogWarning(exception, "SignalR connection closed");
        NotifyConnectionStateChanged(HubConnectionState.Disconnected);
        return Task.CompletedTask;
    }

    private void NotifyConnectionStateChanged(HubConnectionState state)
    {
        ConnectionStateChanged?.Invoke(new ConnectionStateChangedEventArgs
        {
            State = state,
            Timestamp = DateTime.UtcNow
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}

// Event argument classes
public class CommentReceivedEventArgs
{
    public string CommentId { get; set; } = string.Empty;
    public string VideoId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class CommentDeletedEventArgs
{
    public string CommentId { get; set; } = string.Empty;
    public string VideoId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class LikeCountUpdatedEventArgs
{
    public string VideoId { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ViewCountUpdatedEventArgs
{
    public string VideoId { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class UserJoinedEventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string VideoId { get; set; } = string.Empty;
    public int OnlineCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class UserLeftEventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string VideoId { get; set; } = string.Empty;
    public int OnlineCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class OnlineUsersEventArgs
{
    public string VideoId { get; set; } = string.Empty;
    public List<string> Users { get; set; } = new();
    public int Count { get; set; }
}

public class NotificationEventArgs
{
    public string NotificationId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class NewVideoEventArgs
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string UploaderName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ConnectionStateChangedEventArgs
{
    public HubConnectionState State { get; set; }
    public DateTime Timestamp { get; set; }
}
