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

        // Handle server messages (success or rejection)
        _connection.On<string>("ConnectionAccepted", message =>
        {
            Console.WriteLine(message); // Connected successfully
        });

        _connection.On<string>("ConnectionRejected", message =>
        {
            Console.WriteLine(message); // Connection rejected
            // Optionally stop the connection immediately since it's rejected
            Task.Run(async () =>
            {
                await _connection.StopAsync();
            }).Wait();
        });

        _connection.On<string>("ConnectionFailed", message =>
        {
            Console.WriteLine($"Connection failed: {message}");
            // Optionally stop the connection immediately since it's failed
            Task.Run(async () =>
            {
                await _connection.StopAsync();
            }).Wait();
        });

        // Start the connection process
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
            Console.WriteLine("Attempting to connect to the server...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect: {ex.Message}");
        }
    }
}
