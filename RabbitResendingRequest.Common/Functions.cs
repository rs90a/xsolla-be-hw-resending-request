using System;

namespace RabbitResendingRequest.Common
{
    public class Functions
    {
        public static string GetDelayQueueName(int delay) => 
            Constants.RmqDelayQueuePrefix + delay;
        
        public static string GetDelayQueueRoute(int delay) => 
            Constants.RmqDelayQueueRoutePrefix + delay + Constants.RmqDelayQueueRoutePostfix;

        public static void PrintDefaultMessage()
        {
            Console.WriteLine("[Статус Consumer] Ожидание сообщения...");
            Console.WriteLine(@"[Подсказка] Нажмите ""Enter"" для выхода из программы");
        }
    }
}