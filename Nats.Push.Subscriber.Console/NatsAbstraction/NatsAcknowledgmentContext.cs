using NATS.Client;

namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    internal class NatsAcknowledgmentContext(Msg originalMessage) : IAcknowledgmentContext
    {
        private readonly Msg _originalMessage = originalMessage ?? throw new ArgumentNullException(nameof(originalMessage));

        public void Ack()
        {
            _originalMessage.Ack();
        }

        public void InProgress()
        {
            _originalMessage.InProgress();
        }

        public void Nak()
        {
            _originalMessage.Nak();
        }

        public void Term()
        {
            _originalMessage.Term();
        }
    }
}
