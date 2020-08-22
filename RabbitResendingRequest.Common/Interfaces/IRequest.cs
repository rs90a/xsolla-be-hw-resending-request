using RabbitResendingRequest.Common.Models;

namespace RabbitResendingRequest.Common.Interfaces
{
    public interface IRequest
    {
        public void SendRequest(Message message);
    }
}