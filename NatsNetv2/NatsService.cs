using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace NatsNetv2;

public class NatsService : IAsyncDisposable
{
    private readonly NatsConnection _nc;
    private readonly NatsJSContext _js;

    public NatsService()
    {
        _nc = new NatsConnection(new NatsOpts { Url = "nats://nats-server:4222" });
        _js = new NatsJSContext(_nc);
    }

    public async Task CreateStreamAsync(string streamName, string[] subjects)
    {
        await _js.CreateStreamAsync(new StreamConfig(streamName, subjects));
    }

    public async Task CreateConsumerAsync(string streamName, string durableName, string natsSubject)
    {
        await _js.CreateConsumerAsync(streamName, new ConsumerConfig(durableName)
        {
            FilterSubject = natsSubject,
        });
    }

    public ValueTask DisposeAsync() => _nc.DisposeAsync();

    public async Task<ConsumerWrapper> SubscribeTopicAsync(string subjectName, AsyncEventHandler<MessageHandlerEventArgs> handler, string streamName, string durableName)
    {
        var consumer = await _js.GetConsumerAsync(streamName, durableName);
        return new ConsumerWrapper(consumer, handler);
    }

    public async Task PublishMessageAsync(string natsSubject, string messageToSend)
    {
        var ack = await _js.PublishAsync(natsSubject, messageToSend);
        ack.EnsureSuccess();
    }
}

public class ConsumerWrapper
{
    private readonly INatsJSConsumer _consumer;
    private readonly AsyncEventHandler<MessageHandlerEventArgs> _handler;
    private readonly CancellationTokenSource _cts;
    private readonly Task _task;

    public ConsumerWrapper(INatsJSConsumer consumer, AsyncEventHandler<MessageHandlerEventArgs> handler)
    {
        _consumer = consumer;
        _handler = handler;
        _cts = new CancellationTokenSource();
        _task = Task.Run(ConsumeLoop);
    }

    private async Task ConsumeLoop()
    {
        await foreach (var msg in _consumer.ConsumeAsync<string>(cancellationToken: _cts.Token))
        {
            var natsAcknowledgmentContext = new NatsAcknowledgmentContext(msg);
            await _handler(this, new MessageHandlerEventArgs(msg.Data!, msg.Subject, natsAcknowledgmentContext));
        }
    }

    public async Task StopAsync()
    {
        await _cts.CancelAsync();
        await _task;
    }
}