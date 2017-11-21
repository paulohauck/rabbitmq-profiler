namespace RabbitMQ_Profiller
{
    public interface IMessageHandler
    {
        void Close();
        void StartListening();
    }
}