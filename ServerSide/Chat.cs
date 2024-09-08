using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private const int MaxConnectionsPerUser = 2;
    private static readonly ConcurrentDictionary<string, List<ConnectionInfo>> UserConnections = new();

    // Override OnConnectedAsync to assign UserIdentifier based on query string
    public override async Task OnConnectedAsync()
    {
        // Extract userId from the query string
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("Connection failed: User ID is null or empty.");
            await base.OnConnectedAsync();
            return;
        }

        // Assign the userId to Context.UserIdentifier
        Context.Items["UserIdentifier"] = userId;

        Console.WriteLine($"User trying to connect: {userId}, Connection ID: {Context.ConnectionId}");

        // Check if the user has reached the maximum allowed connections
        if (UserConnections.TryGetValue(userId, out var connections) && connections.Count >= MaxConnectionsPerUser)
        {
            Console.WriteLine($"Connection rejected: {userId} has reached the maximum allowed connections.");
            Context.Abort();  // Prevent the connection from being established
            return;
        }

        var newConnection = new ConnectionInfo(Context.ConnectionId, DateTime.UtcNow);
        ManageUserConnections(userId, newConnection);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.Items["UserIdentifier"]?.ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            Console.WriteLine($"User disconnected: {userId}, Connection ID: {Context.ConnectionId}");
            RemoveUserConnection(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private void ManageUserConnections(string userId, ConnectionInfo newConnection)
    {
        UserConnections.AddOrUpdate(userId,
            _ => new List<ConnectionInfo> { newConnection }, // If no connections exist, add the new connection
            (_, connections) =>
            {
                // Add the new connection
                connections.Add(newConnection);
                return connections;
            });
    }

    private void RemoveUserConnection(string userId, string connectionId)
    {
        if (UserConnections.TryGetValue(userId, out var connections))
        {
            // Find the connection to remove
            var connection = connections.FirstOrDefault(c => c.ConnectionId == connectionId);

            if (connection != null)
            {
                // Remove the connection
                connections.Remove(connection);

                // If no connections remain, remove the user from the dictionary
                if (!connections.Any())
                {
                    UserConnections.TryRemove(userId, out _);
                }
            }
        }
    }

    private class ConnectionInfo
    {
        public string ConnectionId { get; }
        public DateTime ConnectedAt { get; }

        public ConnectionInfo(string connectionId, DateTime connectedAt)
        {
            ConnectionId = connectionId;
            ConnectedAt = connectedAt;
        }
    }
}
