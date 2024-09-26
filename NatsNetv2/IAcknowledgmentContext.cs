namespace NatsNetv2;

public interface IAcknowledgmentContext
{
    ValueTask AckAsync();

    ValueTask NakAsync();

    ValueTask TermAsync();

    ValueTask InProgressAsync();
}