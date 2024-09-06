using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5281/chathub")  // Update to your SignalR hub URL
            .Build();

        // Reconnect on disconnection
        connection.Closed += async (error) =>
        {
            Console.WriteLine("Connection closed. Reconnecting...");
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

        // Receive messages from the server
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine($"{user}: {message}");
        });

        try
        {
            // Start the connection
            await connection.StartAsync();
            Console.WriteLine("Connection started.");

            // Simulating joining a group
            Console.Write("Enter your username: ");
            string userName = Console.ReadLine();
            string groupName = "YourGroupName";  // You can prompt this as well or use a specific group name

            // Join the group
            await connection.InvokeAsync("JoinGroup", groupName, userName);
            Console.WriteLine($"Joined group '{groupName}' as {userName}.");

            while (true)
            {
                // Options for user actions
                Console.WriteLine("\n1. Send message to group");
                Console.WriteLine("2. Get usernames in group");
                Console.WriteLine("3. Get user count in group");
                Console.WriteLine("4. Leave group");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.Write("Enter your message: ");
                        string message = Console.ReadLine();
                        await connection.InvokeAsync("SendMessageToGroup", groupName, userName, message);
                        break;

                    case "2":
                        var usernamesInGroup = await connection.InvokeAsync<List<string>>("GetUsernamesInGroup", groupName);
                        Console.WriteLine($"Usernames in group '{groupName}': {string.Join(", ", usernamesInGroup)}");
                        break;

                    case "3":
                        var userCount = await connection.InvokeAsync<int>("GetUserCountInGroup", groupName);
                        Console.WriteLine($"User count in group '{groupName}': {userCount}");
                        break;

                    case "4":
                        await connection.InvokeAsync("LeaveGroup", groupName, userName);
                        Console.WriteLine($"You have left the group '{groupName}'.");
                        break;

                    case "5":
                        // Gracefully close the connection before exiting
                        await connection.StopAsync();
                        Console.WriteLine("Connection closed.");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
