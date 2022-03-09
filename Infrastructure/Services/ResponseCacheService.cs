using System.Text;
using System.Text.Json;
using Core.Interfaces;
using Core.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.Services;

public class ResponseCacheService : IResponseCacheService
{
    public readonly IDatabase Database;
    private IModel _channel;
    private const float TIME_TO_LIVE = 1000f;
    
    public ResponseCacheService(IConnectionMultiplexer redis)
    {
        Database = redis.GetDatabase();
        
        var factory = new ConnectionFactory {Uri = new Uri("amqp://guest:guest@localhost:5672")};
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        _channel = channel;
        _channel.QueueDeclare("demo-queue", durable: true, exclusive: false, autoDelete:false, arguments: null);
    }

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        var message = new MessageObj()
        {
            CatchKey = cacheKey,
            Response = response,
            TimeToLive = timeToLive
        };
        
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        
        _channel.BasicPublish("", "demo-queue", null, body);
    }

    public async Task<string> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await Database.StringGetAsync(cacheKey);

        if (cachedResponse.IsNullOrEmpty)
        {
            return null;
        }

        return cachedResponse;
    }
}