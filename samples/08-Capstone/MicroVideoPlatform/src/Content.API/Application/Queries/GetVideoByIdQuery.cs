using Content.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Content.API.Application.Queries;

public record GetVideoByIdQuery(Guid Id) : IRequest<VideoDto?>;

public record VideoDto(Guid Id, string Title, string Url, string Status, DateTime UploadedAt, int ViewCount);

public class GetVideoByIdQueryHandler : IRequestHandler<GetVideoByIdQuery, VideoDto?>
{
    private readonly VideoDbContext _context;

    public GetVideoByIdQueryHandler(VideoDbContext context)
    {
        _context = context;
    }

    public async Task<VideoDto?> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (video == null)
            return null;

        return new VideoDto(
            video.Id,
            video.Title,
            video.Url,
            video.Status.ToString(),
            video.UploadedAt,
            video.ViewCount
        );
    }
}

public record GetAllVideosQuery : IRequest<List<VideoDto>>;

public class GetAllVideosQueryHandler : IRequestHandler<GetAllVideosQuery, List<VideoDto>>
{
    private readonly VideoDbContext _context;

    public GetAllVideosQueryHandler(VideoDbContext context)
    {
        _context = context;
    }

    public async Task<List<VideoDto>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
    {
        var videos = await _context.Videos.ToListAsync(cancellationToken);

        return videos.Select(v => new VideoDto(
            v.Id,
            v.Title,
            v.Url,
            v.Status.ToString(),
            v.UploadedAt,
            v.ViewCount
        )).ToList();
    }
}
