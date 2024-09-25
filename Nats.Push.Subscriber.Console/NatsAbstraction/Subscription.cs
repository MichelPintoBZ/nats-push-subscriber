using NATS.Client.JetStream;

namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    internal class Subscription(IJetStreamPushAsyncSubscription subscription) : ISubscription
    {
        private readonly IJetStreamPushAsyncSubscription _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));

        public string SubjectName => _subscription.Subject;

        public void Unsubscribe()
        {
            _subscription.Unsubscribe();
        }
    }
}
