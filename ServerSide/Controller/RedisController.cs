using Microsoft.AspNetCore.Mvc;
using Chat.Redis;
using StackExchange.Redis;
namespace Redis.Controllers
{
    [Controller]
    public abstract class RedisController : ControllerBase
    {
        private readonly RedisService _redisService;
        protected IDatabase RedisReadDb { get; }
        protected IDatabase RedisWriteDb { get; }

        public RedisController(RedisService redisService)
        {
            _redisService = redisService;
            RedisReadDb = _redisService.GetRedisReadDatabase();
            RedisWriteDb = _redisService.GetRedisWriteDatabase();
        }

        // Write operations (e.g., adding a user to a group)
        public async Task AddUserToGroupAsync(string groupName, string userId)
        {
            await RedisWriteDb.SetAddAsync($"group:{groupName}:users", userId);
        }

        // Increment user count (write operation)
        public async Task IncrementUserCountAsync(string groupName)
        {
            await RedisWriteDb.StringIncrementAsync($"group:{groupName}:count");
        }

        // Read operations (e.g., getting user count)
        public async Task<long> GetUserCountInGroupAsync(string groupName)
        {
            return (long)await RedisReadDb.StringGetAsync($"group:{groupName}:count");
        }

        // Read operation: Check if a user is in a group
        public async Task<bool> IsUserInGroupAsync(string groupName, string userId)
        {
            return await RedisReadDb.SetContainsAsync($"group:{groupName}:users", userId);
        }

        // Remove a user from a group (write operation)
        public async Task RemoveUserFromGroupAsync(string groupName, string userId)
        {
            await RedisWriteDb.SetRemoveAsync($"group:{groupName}:users", userId);
        }

        // Decrement user count (write operation)
        public async Task DecrementUserCountAsync(string groupName)
        {
            await RedisWriteDb.StringDecrementAsync($"group:{groupName}:count");
        }

        // Reading all groups for a user
        public async Task<RedisValue[]> GetAllGroupsForUserAsync(string userId)
        {
            var groupKeys = RedisReadDb.Multiplexer.GetServer(RedisReadDb.Multiplexer.GetEndPoints()[0]).Keys(pattern: $"group:*:users");
            var userGroups = new List<RedisValue>();

            foreach (var key in groupKeys)
            {
                if (await RedisReadDb.SetContainsAsync(key, userId))
                {
                    userGroups.Add(key.ToString());
                }
            }

            return userGroups.ToArray();
        }
    }

}