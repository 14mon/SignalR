using Chat.Hubs;
using Chat.Redis;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
// string? redisReadConnectionString = Environment.GetEnvironmentVariable("REDIS_READ_CONNECTION_STRING");
// string? redisWriteConnectionString = Environment.GetEnvironmentVariable("REDIS_WRITE_CONNECTION_STRING");
// string? redisInstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME");

// builder.Services.AddSingleton(sp => new RedisService(redisReadConnectionString!, redisWriteConnectionString!, redisInstanceName!));
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
});

app.Run();
