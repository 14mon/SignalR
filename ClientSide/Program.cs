using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5281/chathub")
            .Build();

        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"{user}: {message}");
        });

        await hubConnection.StartAsync();

        Console.WriteLine("Connection started. Type a message and press Enter to send:");

        while (true)
        {
            var user = "ConsoleApp";
            var message = Console.ReadLine();
            await hubConnection.SendAsync("SendMessage", user, message);
        }
    }
}
