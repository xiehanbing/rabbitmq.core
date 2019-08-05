using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace RabbitMq.Core.Consume
{
    public class ChapterLister: RabbitListener
    {
        private readonly ILogger<RabbitListener> _logger;
        // 因为Process函数是委托回调,直接将其他Service注入的话两者不在一个scope,
        // 这里要调用其他的Service实例只能用IServiceProvider CreateScope后获取实例对象
        private readonly IServiceProvider _services;
        public ChapterLister(IServiceProvider services,IOptions<AppConfiguration>options,
            ILogger<RabbitListener>logger) : base(options)
        {
            _logger = logger;
            _services = services;
            base.QueueName = "exchange_test";
            base.QueueName = "queue.test";
            base.RabbitType = "topic";
            base.RouteKey = "key.test.zero";

        }

        public override bool Process(string message)
        {
            Console.WriteLine($"ChapterLister Process:{message}");
            //var taskMessage = JToken.Parse(message);
            //if (taskMessage == null)
            //{
            //    //返回false 的时候 会直接驳回此消息 ，标示处理不了
            //    return false;
            //}

            try
            {
                using (var scope=_services.CreateScope())
                {
                    Console.WriteLine($"message is :{message}");
                    //做其他操作
                    //var service = scope.ServiceProvider.GetRequiredService<>();
                    return true;
                }
            }
            catch (Exception ex)
            {
               _logger.LogInformation($"Process fail ChapterLister,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
                _logger.LogError(-1,ex, "Process fail ChapterLister");
                return false;
            }
        }
    }
}