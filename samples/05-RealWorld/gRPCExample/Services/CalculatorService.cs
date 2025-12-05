using Grpc.Core;
using GrpcExample;

namespace gRPCExample.Services;

public class CalculatorService : Calculator.CalculatorBase
{
    public override Task<CalculateReply> Add(CalculateRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CalculateReply { Result = request.A + request.B });
    }

    public override Task<CalculateReply> Multiply(CalculateRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CalculateReply { Result = request.A * request.B });
    }
}
