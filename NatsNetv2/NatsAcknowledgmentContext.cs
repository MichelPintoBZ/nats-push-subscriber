using NATS.Client.JetStream;

namespace NatsNetv2;

internal class NatsAcknowledgmentContext(NatsJSMsg<string> originalMessage) : IAcknowledgmentContext
{
    public ValueTask AckAsync() => originalMessage.AckAsync();

    public ValueTask InProgressAsync() => originalMessage.AckProgressAsync();

    public ValueTask NakAsync() => originalMessage.NakAsync();

    public ValueTask TermAsync() => originalMessage.AckTerminateAsync();
}