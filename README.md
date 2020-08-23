## Описание
Xsolla Summer School 2020 BE. Домашнее задание по RabbitMQ. 

Реализация Producer, Consumer с механизмом переотправки запроса в случае неудачной попытки (HTTP-код < 200 и HTTP-код >= 210).

## Особенности реализации механизм переотправки запроса
Описание приводится в рамках [файла с константами](https://github.com/rs90a/xsolla-be-hw-resending-request/blob/master/RabbitResendingRequest.Common/Constants.cs).

1. Одна точка обмена (**RmqExchangeName**) типа **Direct**
1. Одна очередь для сообщений (**RmqMsgQueue**) с маршрутом **RmqMsgQueueRoute**
1. N-очередей для каждого временного интервала (в мс) переотправки запроса, где N - кол-во значений задержек в **RmqDelays**. Данные очереди создаются с указанием дополнительных параметров:
     - `x-dead-letter-exchange` = `RmqExchangeName`
     - `x-dead-letter-routing-key` = `RmqMsgQueue`
     - `x-message-ttl` = `значение задержки из RmqDelays`
     
     Данная конфигурация означает, что сообщения в такой очереди могут находиться только определенное время (`x-message-ttl`). Если время нахождения сообщения в очереди истекает, то данное сообщение перемещается в точку обмена `x-dead-letter-exchange` в очередь по маршруту `x-dead-letter-routing-key`. В случае, если бы параметры `x-dead-letter-exchange` и  `x-dead-letter-routing-key` не были заданы, то сообщение просто бы потерялось, т.е. было бы удалено из очереди.
1. Наличие ограничения по количеству выполнения повторных запросов (**AttempNumMax**)
1. Как же это работает? Изначально [**Consumer**](https://github.com/rs90a/xsolla-be-hw-resending-request/blob/master/RabbitResendingRequest.Common/MessageConsumer.cs) подписан на получение сообщений из очереди **RmqMsgQueue**. В первый раз [**Producer**](https://github.com/rs90a/xsolla-be-hw-resending-request/blob/master/RabbitResendingRequest.Common/MessageProducer.cs) посылает сообщение в точку обмена **RmqExchangeName** по маршруту **RmqMsgQueueRoute**, что означает, что сообщение попадет в очереди по маршруту **RmqMsgQueueRoute**, в данном случае - в очередь **RmqMsgQueue**. Далее **Consumer** получает сообщение из очереди **RmqMsgQueue** и пытается выполнить GET-запрос с полученными данными. Если запрос завершился неудачей (HTTP-код < 200 и HTTP-код >= 210), то **Producer** выполняет отправку сообщения в очередь соответствующего временного интервала. По истечению времени задержки, сообщение из очереди временного интервала перемещается в основную очередь **RmqMsgQueue**. Далее все действия повторяются - срабатывает **Consumer** на получение сообщение из очереди **RmqMsgQueue**, выполняется GET-запрос, выполняется проверка HTTP-кода, принимается решение о повторной отправки запроса и т.д.

## Требования
* .NET Core 3.1
* RabbitMQ

## Запуск

Запустить два приложения из директории решения:

1. `dotnet run --project RabbitResendingRequest.Сonsumer`
1. `dotnet run --project RabbitResendingRequest.Producer`
