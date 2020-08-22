namespace RabbitResendingRequest.Common
{
    public class Constants
    {
        //Exchanges
        public const string RmqExchangeName = "data_exchange";
        
        //Queues
        public const string RmqMsgQueue = "messages";
        
        public const string RmqDelayQueuePrefix = "messages_delay_";
        
        //Routes
        public const string RmqMsgQueueRoute = "message_route";
        
        public const string RmqDelayQueueRoutePrefix = "message_delay_";

        public const string RmqDelayQueueRoutePostfix = "_route";
        
        //Delays
        public static readonly int[] RmqDelays = {1000, 5000, 10000};
        
        //Other
        public const int AttempNumMax = 5;
    }
}