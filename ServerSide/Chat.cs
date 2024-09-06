using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Chat.Hubs;
public class ChatHub : Hub
{
    // Dictionary to track group membership (groupName -> List of users in the form (connectionId, userName))
    private static Dictionary<string, List<(string ConnectionId, string UserName)>> GroupUsers = new Dictionary<string, List<(string, string)>>();

    // Method to join a group
    public async Task JoinGroup(string groupName, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        if (!GroupUsers.ContainsKey(groupName))
        {
            GroupUsers[groupName] = new List<(string, string)>();
        }

        if (!GroupUsers[groupName].Any(u => u.ConnectionId == Context.ConnectionId))
        {
            GroupUsers[groupName].Add((Context.ConnectionId, userName));
        }

        await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{userName} has joined the group.");
    }

    // Method to send a message to the group
    public async Task SendMessageToGroup(string groupName, string userName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", userName, message);
    }

    // Method to get usernames in a group
    public Task<List<string>> GetUsernamesInGroup(string groupName)
    {
        if (GroupUsers.ContainsKey(groupName))
        {
            var usernames = GroupUsers[groupName].Select(u => u.UserName).ToList();
            return Task.FromResult(usernames);
        }
        return Task.FromResult(new List<string>());
    }

    // Method to get user count in a group
    public Task<int> GetUserCountInGroup(string groupName)
    {
        if (GroupUsers.ContainsKey(groupName))
        {
            return Task.FromResult(GroupUsers[groupName].Count);
        }
        return Task.FromResult(0);
    }

    // Handle when users disconnect and remove them from groups
    public override async Task OnDisconnectedAsync(System.Exception exception)
    {
        var groupsToRemoveFrom = new List<string>();

        foreach (var group in GroupUsers)
        {
            var user = group.Value.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != default)
            {
                group.Value.Remove(user);
                await Clients.Group(group.Key).SendAsync("ReceiveMessage", "System", $"{user.UserName} has left the group.");

                if (!group.Value.Any()) // If no more users in the group, mark for deletion
                {
                    groupsToRemoveFrom.Add(group.Key);
                }
            }
        }

        // Remove empty groups
        foreach (var groupName in groupsToRemoveFrom)
        {
            GroupUsers.Remove(groupName);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Method to leave a group
    public async Task LeaveGroup(string groupName, string userName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        if (GroupUsers.ContainsKey(groupName))
        {
            var user = GroupUsers[groupName].FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != default)
            {
                GroupUsers[groupName].Remove(user);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{userName} has left the group.");
            }

            if (!GroupUsers[groupName].Any())
            {
                GroupUsers.Remove(groupName);
            }
        }
    }
}
