using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Core
{
    public class RabbitListener : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        /// <summary>
        /// RabbitListener
        /// </summary>
        /// <param name="options">IOptions</param>
        public RabbitListener(IOptions<AppConfiguration> options)
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
                this._connection = factory.CreateConnection();
                this._channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// routekey
        /// </summary>
        protected string RouteKey;
        /// <summary>
        /// 队列名称
        /// </summary>
        protected string QueueName;

        protected string Exchange= "message";
        protected string RabbitType = "topic";
        protected string VirtualHost = "";
        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public virtual bool Process(string message)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 注册消费者监听在这里
        /// </summary>
        public void Register()
        {
            Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
            _channel.ExchangeDeclare(exchange: Exchange, type: RabbitType);
            _channel.QueueDeclare(queue: QueueName, exclusive: false);
            _channel.QueueBind(queue:QueueName,exchange: Exchange, routingKey:RouteKey);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);
                if (result)
                {
                    _channel.BasicAck(ea.DeliveryTag,false);
                }
            };
            _channel.BasicConsume(queue: QueueName, consumer: consumer);
        }
        /// <summary>
        /// 关闭注册
        /// </summary>
        public void DeRegister()
        {
            this._connection.Close();
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
           Register();
            return Task.CompletedTask;
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._connection.Close();
            return Task.CompletedTask;
        }
    }
}