using SignalRExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.MapGet("/", () => Results.Text("""
<!DOCTYPE html>
<html>
<head>
    <title>SignalR Chat</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js"></script>
</head>
<body>
    <h1>💬 SignalR Chat</h1>
    <div>
        <input type="text" id="userInput" placeholder="Your name" />
        <input type="text" id="messageInput" placeholder="Message" />
        <button onclick="sendMessage()">Send</button>
    </div>
    <ul id="messagesList"></ul>

    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .build();

        connection.on("ReceiveMessage", (user, message) => {
            const li = document.createElement("li");
            li.textContent = `${user}: ${message}`;
            document.getElementById("messagesList").appendChild(li);
        });

        connection.start().catch(err => console.error(err));

        function sendMessage() {
            const user = document.getElementById("userInput").value;
            const message = document.getElementById("messageInput").value;
            connection.invoke("SendMessage", user, message).catch(err => console.error(err));
            document.getElementById("messageInput").value = "";
        }
    </script>
</body>
</html>
""", "text/html"));

app.Run();
