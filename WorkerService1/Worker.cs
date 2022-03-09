using System.Text;
using Core.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WorkerService1;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDatabase _database;
    private readonly IModel _channel;

    public Worker(ILogger<Worker> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        
        _database = redis.GetDatabase();

        var factory = new ConnectionFactory {Uri = new Uri("amqp://guest:guest@localhost:5672")};
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        _channel = channel;
        _channel.QueueDeclare("demo-queue", durable: true, exclusive: false, autoDelete:false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MessageObj theMessageObject = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                theMessageObject = JsonConvert.DeserializeObject<MessageObj>(message);
            };

            _channel.BasicConsume("demo-queue", true, consumer);

            Console.WriteLine(theMessageObject != null);
            if (theMessageObject != null)
            {
                string serializedResponse = JsonConvert.SerializeObject(theMessageObject.Response);

                await _database.StringSetAsync(theMessageObject.CatchKey, serializedResponse, theMessageObject.TimeToLive);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}