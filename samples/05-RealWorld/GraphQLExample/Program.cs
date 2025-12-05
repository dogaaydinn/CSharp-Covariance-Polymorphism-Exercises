var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class Query
{
    public Book GetBook() => new(1, "Clean Code", "Robert C. Martin");

    public List<Book> GetBooks() => new()
    {
        new(1, "Clean Code", "Robert C. Martin"),
        new(2, "Design Patterns", "Gang of Four"),
        new(3, "Refactoring", "Martin Fowler")
    };
}

public record Book(int Id, string Title, string Author);
