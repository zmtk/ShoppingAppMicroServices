using System.Text;
using System.Text.Json;
using CatalogApi.Dtos;
using RabbitMQ.Client;

namespace CatalogApi.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{

    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        const int maxRetryAttempts = 5;
        int currentRetryAttempt = 0;

        while (currentRetryAttempt < maxRetryAttempts)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]!)
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown!;

                Console.WriteLine("--> Connected to MessageBus");

                // Connection successful, break out of the loop
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");

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
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
        );

        Console.WriteLine($"--> We have sent {message}");

    }
    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if (_channel!.IsOpen)
        {
            _channel.Close();
            _connection!.Close();
        }
    }

    public void UpdateBasketPrices(UpdateBasketPriceDto updateBasketPricesDto)
    {
        var message = JsonSerializer.Serialize(updateBasketPricesDto);

        if (_connection!.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Conneciton Open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection is Closed.");
        }
    }

    public void ToggleProductFromBasket(ToggleProductFromBasketDto toggleProductFromBasketDto)
    {
        var message = JsonSerializer.Serialize(toggleProductFromBasketDto);

        if (_connection!.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Conneciton Open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection is Closed.");
        }
    }
}
