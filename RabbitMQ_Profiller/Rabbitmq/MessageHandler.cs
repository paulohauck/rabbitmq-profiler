using LiteDB;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        json.Add("AMQP_Exchange", ea.Exchange);
                        json.Add("AMQP_ReceivedAt", DateTime.Now.ToString());

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

            using (var db = new LiteDatabase(@"MessageProfiller.db"))
            {
                var messages = db.GetCollection("messages");                
                messages.Insert(new BsonDocument(ToDictionary(json)));
            }
        }

        private Dictionary<string, BsonValue> ToDictionary(JToken token)
        {
            var dict = token.ToObject<Dictionary<string, BsonValue>>();
            foreach (var item in dict.Where(x => x.Value.IsNull).ToList())
            {
                dict[item.Key] = new BsonValue(ToDictionary(token[item.Key]));
            }

            if (!dict.ContainsKey("_id"))
            {
                dict.Add("_id", Guid.NewGuid());
            }

            return dict;
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
