namespace RabbitMq.Core
{
    /// <summary>
    /// app config  
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// rabbithost
        /// </summary>
        public string  RabbitHost { get; set; }
        /// <summary>
        /// rabbit 登录名
        /// </summary>
        public string  RabbitUserName { get; set; }
        /// <summary>
        /// rabbit 登录密码
        /// </summary>
        public string  RabbitPassword { get; set; }
        /// <summary>
        /// rabbit 端口号
        /// </summary>
        public int  RabbitPort { get; set; }
    }
}