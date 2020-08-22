using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitResendingRequest.Common.Interfaces;
using RabbitResendingRequest.Common.Models;

namespace RabbitResendingRequest.Common
{
    public class MessageHandler
    {
        private readonly IRequest request;
        
        public MessageHandler(IRequest request) =>
            this.request = request;

        public EventHandler<BasicDeliverEventArgs> Create()
        {
            return (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var receivedData = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<Message>(receivedData);
                Console.WriteLine($"[Статус Consumer] Получено сообщение: {message.Uri}");
                request.SendRequest(message);
            };
        }
    }
}