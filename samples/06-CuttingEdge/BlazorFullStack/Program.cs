using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<TodoService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// API endpoints
app.MapGet("/api/todos", (TodoService service) => service.GetTodos());
app.MapPost("/api/todos", (TodoItem todo, TodoService service) => service.AddTodo(todo));

app.Run();

public class TodoService
{
    private readonly List<TodoItem> _todos = new()
    {
        new TodoItem { Id = 1, Title = "Learn Blazor", IsComplete = false },
        new TodoItem { Id = 2, Title = "Build an app", IsComplete = false }
    };

    public List<TodoItem> GetTodos() => _todos;
    public void AddTodo(TodoItem todo)
    {
        todo.Id = _todos.Max(t => t.Id) + 1;
        _todos.Add(todo);
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}
