using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctions;

public class ImageResizeFunction
{
    private readonly ILogger<ImageResizeFunction> _logger;

    public ImageResizeFunction(ILogger<ImageResizeFunction> logger)
    {
        _logger = logger;
    }

    [Function("ImageResize")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Processing image resize request");

        // Simulate image processing
        await Task.Delay(100);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            message = "Image resized successfully",
            originalSize = "1920x1080",
            newSize = "800x600",
            processingTime = "100ms"
        });

        return response;
    }

    [Function("HealthCheck")]
    public HttpResponseData HealthCheck(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("Healthy");
        return response;
    }
}
