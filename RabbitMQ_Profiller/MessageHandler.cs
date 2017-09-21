using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ_Profiller
{
    public class MessageHandler : IMessageHandler
    {

        private static IConnection _connection;
        private static IModel _channel;

        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionFactory _connectionFactory;

        public MessageHandler(IServiceProvider serviceProvider, ConnectionFactory connectionFactory)
        {
            _serviceProvider = serviceProvider;
            _connectionFactory = connectionFactory;

            // Set the Automatic Recovery to avoid loss of connection due to inactivity or network problems.
            _connectionFactory.AutomaticRecoveryEnabled = true;
        }

        public void StartListening()
        {
            Connect();

            var eventName = "amq.rabbitmq.trace";

            //_channel.ExchangeDeclare(exchange: eventName, type: "direct");

            var queueName = $"RabbitMQ_Profiller";
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: eventName, routingKey: "#");

            var eventingBasicConsumer = new EventingBasicConsumer(_channel);

            eventingBasicConsumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        var json = JObject.Parse(message);

                        if (json.HasValues)
                        {
                            try
                            {
                                SaveMessage(json);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: eventingBasicConsumer);
        }

        private void SaveMessage(JObject json)
        {
            //var db = null;
            Console.WriteLine($"{ DateTime.Now } : Message Received: " +
                $"\n \t {json.ToString()}");
        }

        public void Close()
        {
            _channel.Close(200, "Goodbye");
            _connection.Close();
            _connection = null;
        }

        private void Connect()
        {
            if (_connection != null)
            {
                return;
            }

            var tries = 0;

            while (tries < 5)
            {
                try
                {
                    _connection = _connectionFactory.CreateConnection();
                    _channel = _connection.CreateModel();
                    return;
                }
                catch (Exception)
                {
                    tries++;
                    Thread.Sleep(15000);
                }
            }
        }

    }
}
