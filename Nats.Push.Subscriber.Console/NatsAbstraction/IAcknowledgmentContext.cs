namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    public interface IAcknowledgmentContext
    {
        void Ack();

        void Nak();

        void Term();

        void InProgress();
    }
}
