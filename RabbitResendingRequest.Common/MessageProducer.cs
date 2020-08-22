using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitResendingRequest.Common.Models;

namespace RabbitResendingRequest.Common
{
    public class MessageProducer
    {
        private readonly string exchangeName;
        private readonly IModel channel;
        
        public MessageProducer(IModel channel, string exchangeName)
        {
            this.channel = channel;
            this.exchangeName = exchangeName;
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        }

        public void Send(string msgData, string route)
        {
            var message = GetMessage(msgData);
            var props = channel.CreateBasicProperties();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: exchangeName, route, basicProperties: props, body: body);
            Console.WriteLine($"[Статус Producer] Сообщение отправлено: {msgData}");
        }
        
        private Message GetMessage(string uri) => new Message { Uri = uri};
    }
}