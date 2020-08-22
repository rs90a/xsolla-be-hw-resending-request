using RabbitMQ.Client;
using RabbitResendingRequest.Common;
using Constants = RabbitResendingRequest.Common.Constants;

namespace RabbitResendingRequest.Producer
{
    class Program
    {
        private const string msgData = "https://google.com/";

        static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            {
                using var channel = connection.CreateModel();
                {
                    var messageProducer = new MessageProducer(channel, Constants.RmqExchangeName);
                    messageProducer.Send(msgData, Constants.RmqMsgQueueRoute);
                }
            }
        }
    }
}