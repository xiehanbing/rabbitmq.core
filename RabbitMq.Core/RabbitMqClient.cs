using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMq.Core
{
    public class RabbitMqClient
    {
        private readonly IModel _channel;
        private readonly ILogger _logger;

        public RabbitMqClient(IOptions<AppConfiguration> options,
            ILogger<RabbitMqClient> logger)
        {
            try
            {
                var factory=new ConnectionFactory()
                {
                    HostName = options.Value.RabbitHost,
                    UserName = options.Value.RabbitUserName,
                    Password = options.Value.RabbitPassword,
                    Port = options.Value.RabbitPort
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
        }
        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="routingKey">routingKey</param>
        /// <param name="message">message</param>
        /// <param name="queue">queue</param>
        /// <param name="exchange">exchange</param>
        public virtual void PushMessage(string routingKey, object message, string queue="message",string exchange="message")
        {
            _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
            _channel.QueueDeclare(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}