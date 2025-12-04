using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MicroVideoPlatform.Shared.Contracts;
using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;
using MicroVideoPlatform.Shared.Events;
using MicroVideoPlatform.Shared.Common;

namespace MicroVideoPlatform.Content.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly IVideoRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VideosController> _logger;

    public VideosController(IVideoRepository repository, IEventBus eventBus, IDistributedCache cache, ILogger<VideosController> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoDto>>> GetAll([FromQuery] VideoStatus? status, [FromQuery] string? category, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var cacheKey = $"{Constants.CacheKeys.VideosAllPrefix}{status}:{category}:{skip}:{take}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            var videos = JsonSerializer.Deserialize<IEnumerable<VideoDto>>(cached);
            return Ok(videos);
        }

        var result = await _repository.GetAllAsync(status, category, skip, take);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Constants.CacheExpiration.VideosList)
        });

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VideoDto>> GetById(Guid id)
    {
        var cacheKey = Constants.CacheKeys.Video(id);
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            var video = JsonSerializer.Deserialize<VideoDto>(cached);
            return Ok(video);
        }

        var result = await _repository.GetByIdAsync(id);
        if (result == null) return NotFound();

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Constants.CacheExpiration.Video)
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<VideoDto>> Create([FromBody] VideoDto dto)
    {
        var created = await _repository.CreateAsync(dto);

        await _eventBus.PublishAsync(new VideoUploadedEvent
        {
            VideoId = created.Id,
            Video = created,
            FilePath = $"/app/uploads/{created.FileName}",
            UploadedBy = created.UploadedBy ?? "anonymous",
            RequiresProcessing = true,
            ProcessingPriority = 5
        });

        _logger.LogInformation("Video {VideoId} created and VideoUploadedEvent published", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VideoDto>> Update(Guid id, [FromBody] VideoDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");

        var updated = await _repository.UpdateAsync(dto);
        await _cache.RemoveAsync(Constants.CacheKeys.Video(id));

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound();

        await _cache.RemoveAsync(Constants.CacheKeys.Video(id));
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<VideoDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query parameter is required");

        var results = await _repository.SearchAsync(query);
        return Ok(results);
    }

    [HttpPost("{id:guid}/view")]
    public async Task<IActionResult> IncrementView(Guid id)
    {
        await _repository.IncrementViewCountAsync(id);
        await _cache.RemoveAsync(Constants.CacheKeys.Video(id));
        return Ok();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount([FromQuery] VideoStatus? status)
    {
        var count = await _repository.GetCountAsync(status);
        return Ok(count);
    }
}
