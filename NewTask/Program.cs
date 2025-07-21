using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

class Program
{

    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
        };
        int retryCount = 0;
        while (retryCount < 5)
        {

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync("task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);



                var message = GetMessages(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties
                {
                    Persistent = true // Make sure the messages are persistent
                };

                await channel.BasicPublishAsync(exchange: "", routingKey: "task_queue", body: body);

                Console.WriteLine($" [x] Sent {message}");
                break;
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($" [!] Broker unreachable: {ex.Message}");
                retryCount++;
                await Task.Delay(5000); // Wait before retrying
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] An error occurred on producer: {ex.Message}");
                break;
            }
        }
    }

        static string GetMessages(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    
}