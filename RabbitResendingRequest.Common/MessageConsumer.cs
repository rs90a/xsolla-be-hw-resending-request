using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitResendingRequest.Common
{
    public class MessageConsumer
    {
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string msgQueueName;
        private readonly string msgRoute;
        private readonly int[] delays;
        
        public MessageConsumer(IModel channel, string exchangeName, string msgQueueName, string msgRoute, int[] delays)
        {
            this.channel = channel;
            this.exchangeName = exchangeName;
            this.msgQueueName = msgQueueName;
            this.msgRoute = msgRoute;
            this.delays = delays;
        }

        private void InitQueue(string queueName, string routingKey, Dictionary<string, object> args = null)
        {
            channel.QueueDeclare(queueName, 
                false, 
                false, 
                false, 
                args);
            channel.QueueBind(queueName, exchangeName, routingKey);
        }
        
        private void InitQueuePool()
        {
            InitQueue(msgQueueName, msgRoute);
            
            var args = new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = exchangeName,
                ["x-dead-letter-routing-key"] = msgRoute
            };
            
            foreach (var delay in delays)
            {
                args["x-message-ttl"] = delay;
                InitQueue(
                    Functions.GetDelayQueueName(delay),
                    Functions.GetDelayQueueRoute(delay),
                    args
                );
            }
        }

        public void Receive()
        {
            InitQueuePool();
            var messageProducer = new MessageProducer(channel, Constants.RmqExchangeName);
            var request = new Request(messageProducer, delays, Constants.AttempNumMax);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += new MessageHandler(request).Create();
            Functions.PrintDefaultMessage();
            channel.BasicConsume(Constants.RmqMsgQueue, true, consumer);
            Console.ReadLine();
        }
    }
}