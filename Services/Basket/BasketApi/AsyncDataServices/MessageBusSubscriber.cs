using System.Text;
using BasketApi.EventrProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BasketApi.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitRabbitMq();
    }

    private void InitRabbitMq()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        const int maxRetryAttempts = 5;
        int currentRetryAttempt = 0;

        while (currentRetryAttempt < maxRetryAttempts)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                // Add Trigger to Configuration.
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName,
                                   exchange: "trigger",
                                   routingKey: "");

                Console.WriteLine("--> Listening On the Message Bus");
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                // Connection successful, break out of the loop
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to message bus. {ex.Message}");

                // Increment the retry attempt
                currentRetryAttempt++;

                // Calculate the delay before the next retry (you can adjust this logic)
                int retryIntervalInSeconds = 5 * currentRetryAttempt;

                Console.WriteLine($"--> Retrying in {retryIntervalInSeconds} seconds...");

                // Sleep for the calculated interval before the next retry
                Thread.Sleep(retryIntervalInSeconds * 1000);
            }
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // Handle connection shutdown events if needed
        Console.WriteLine("--> RabbitMQ connection shutdown");
    }

    public override void Dispose()
    {
        if (_channel != null && _channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (ModuleHandle, ea) =>
        {
            Console.WriteLine("--> Event Received!");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            await _eventProcessor.ProcessEventAsync(notificationMessage);
        };

        try
        {
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not consume RabbitMQ Event {ex.Message}");
        }

        await Task.CompletedTask;
    }

}