using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

class Program
{
    static async Task Main(string[] args)
    {
        {
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
            };
            int retryCount = 0;
            while (retryCount < 5) { 
            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync("task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // Fair dispatch

                    var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x]Received {message}");

                    int dots = message.Split('.').Length - 1;
                    await Task.Delay(dots * 1000); // Simulate work by delaying based on the number of dots

                    Console.WriteLine(" [x] Done");

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await channel.BasicConsumeAsync(queue: "task_queue", autoAck: false, consumer: consumer);

                Console.WriteLine(" [x] waiting for messages. To exit press CTRL+C");
                await Task.Delay(Timeout.Infinite); // Keep the application running
                    break;
            }
            catch (BrokerUnreachableException ex)
            {
                    retryCount++;
                    Console.WriteLine($" [!] Broker unreachable, retrying... Attempt {retryCount}/5");
                    await Task.Delay(5000); // Wait before retrying
            }
                catch (Exception ex)
            {
                Console.WriteLine($" [!] An error occurred on consumer: {ex.Message}");
                break;
                }
            }
        }
    }
}

