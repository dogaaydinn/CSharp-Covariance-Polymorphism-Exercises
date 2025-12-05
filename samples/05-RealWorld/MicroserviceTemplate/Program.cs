using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapGet("/orders/{id}", async (int id, IMediator mediator) =>
{
    var query = new GetOrderQuery(id);
    var order = await mediator.Send(query);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

app.Run();

// CQRS with MediatR
record GetOrderQuery(int Id) : IRequest<Order?>;

class GetOrderHandler : IRequestHandler<GetOrderQuery, Order?>
{
    public Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // In real app: query database
        return Task.FromResult<Order?>(new Order(request.Id, "Order #" + request.Id, 99.99m));
    }
}

record Order(int Id, string Name, decimal Total);
