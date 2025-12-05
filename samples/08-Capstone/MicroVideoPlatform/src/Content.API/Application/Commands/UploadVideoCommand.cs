using Content.API.Domain;
using Content.API.Domain.Events;
using Content.API.Infrastructure;
using MediatR;

namespace Content.API.Application.Commands;

public record UploadVideoCommand(string Title, string Url) : IRequest<Guid>;

public class UploadVideoCommandHandler : IRequestHandler<UploadVideoCommand, Guid>
{
    private readonly VideoDbContext _context;
    private readonly IEventBus _eventBus;

    public UploadVideoCommandHandler(VideoDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task<Guid> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
    {
        var video = new Video(request.Title, request.Url);

        _context.Videos.Add(video);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish event for processing worker
        await _eventBus.PublishAsync(new VideoUploadedEvent(
            video.Id,
            video.Title,
            video.Url,
            video.UploadedAt
        ));

        return video.Id;
    }
}
