namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    public interface ISubscription
    {
        public string SubjectName { get; }

        void Unsubscribe();
    }
}
