namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    public class MessageHandlerEventArgs(string data, string subject, IAcknowledgmentContext acknowledgmentContext) : EventArgs
    {
        public string Data { get; init; } = data ?? throw new ArgumentNullException(nameof(data));

        public string Topic { get; init; } = subject ?? throw new ArgumentNullException(nameof(subject));

        public IAcknowledgmentContext AcknowledgmentContext { get; init; } = acknowledgmentContext ?? throw new ArgumentNullException(nameof(acknowledgmentContext));
    }
}
