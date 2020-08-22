using RabbitMQ.Client;
using RabbitResendingRequest.Common;
using Constants = RabbitResendingRequest.Common.Constants;

namespace RabbitResendingRequest.Consumer
{
    class Program
    {
        static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            {
                using var channel = connection.CreateModel();
                {
                    var messageConsumer = new MessageConsumer(
                        channel,
                        Constants.RmqExchangeName,
                        Constants.RmqMsgQueue,
                        Constants.RmqMsgQueueRoute,
                        Constants.RmqDelays);
                   messageConsumer.Receive();
                }
            }
        }
    }
}