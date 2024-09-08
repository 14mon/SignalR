using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    private static HubConnection _connection;

    static async Task Main(string[] args)
    {
        // Set a userId for the client (you can replace this with actual logic)
        var userId = "123"; // Replace with actual userId logic if needed

        _connection = new HubConnectionBuilder()
            .WithUrl($"http://localhost:5281/chathub?userId={userId}") // Pass userId here
            .WithAutomaticReconnect()
            .Build();

        // Handle server messages (Optional: handle disconnection or other messages from the server)
        _connection.On("ForceDisconnect", async () =>
        {
            Console.WriteLine("You have been disconnected by the server.");
            await _connection.StopAsync();
        });

        await StartConnectionAsync();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }

    private static async Task StartConnectionAsync()
    {
        try
        {
            await _connection.StartAsync();
            Console.WriteLine("Connected to the server.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
        }
    }
}
