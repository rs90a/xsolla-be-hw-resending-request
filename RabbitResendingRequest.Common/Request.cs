using System;
using System.Net.Http;
using RabbitResendingRequest.Common.Interfaces;
using RabbitResendingRequest.Common.Models;

namespace RabbitResendingRequest.Common
{
    public class Request : IRequest
    {
        private int attemptNum;
        private bool isFinished;
        
        private readonly MessageProducer messageProducer;
        private readonly int[] delays;
        private readonly int attempNumMax;

        public Request(MessageProducer messageProducer, int[] delays, int attempNumMax)
        {
            this.messageProducer = messageProducer;
            this.delays = delays;
            this.attempNumMax = attempNumMax;
        }

        public void SendRequest(Message message)
        {
            var httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync(message.Uri).GetAwaiter().GetResult();

            if (IsSuccessStatusCode(httpResponse))
            {
                isFinished = true;
                Console.WriteLine("[Запрос] Выполнение запроса успешно завершено");
                Console.WriteLine($"[Запрос] Результат: {httpResponse}");
            }
            else
            {
                Console.WriteLine("[Запрос] Выполнение запроса завершилось неудачей");
                if (attemptNum == attempNumMax)
                {
                    isFinished = true;
                    Console.WriteLine("[Запрос] Превышено максимальное количество попыток отправки запроса");
                }
                else
                {
                    var curDelay = delays[attemptNum % delays.Length];
                    var curRoute = Functions.GetDelayQueueRoute(curDelay);
                    Console.WriteLine("[Запрос] Выполнение повторной попытки отправки запроса...");
                    Console.WriteLine(@$"[Запрос] Попытка {attemptNum + 1}: отправка сообщения в очередь с routingKey = ""{curRoute}""");
                    attemptNum++;
                    messageProducer.Send(message.Uri, curRoute);
                }
            }

            if (isFinished)
            {
                attemptNum = 0;
                isFinished = false;
                Functions.PrintDefaultMessage();
            }
        }

        private bool IsSuccessStatusCode(HttpResponseMessage httpResponse)
        {
            var statusCode = (int) httpResponse.StatusCode;
            return statusCode >= 200 && statusCode < 210;
        }
    }
}