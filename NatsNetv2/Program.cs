// See https://aka.ms/new-console-template for more information

using NatsNetv2;

Console.WriteLine("Hello, v2!");

string natsSubject = "my.nats.subject.example2";
string streamName = "my-stream2";
string durableName = "my-durable2";

NatsService natsPublishSubscribeService = new();
await natsPublishSubscribeService.CreateStreamAsync(streamName, [natsSubject]);
await natsPublishSubscribeService.CreateConsumerAsync(streamName, durableName, natsSubject);

static async ValueTask newMessageReceivedHandler(object? sender, MessageHandlerEventArgs args)
{
    Console.WriteLine($"Received message2: {args.Data}");

    await args.AcknowledgmentContext.AckAsync();
}

var consumerWrapper = await natsPublishSubscribeService.SubscribeTopicAsync(natsSubject, newMessageReceivedHandler, streamName, durableName);

for (int i = 0; i < 100; i++)
{
    string messageToSend = $"Message number {i}!";

    await natsPublishSubscribeService.PublishMessageAsync(natsSubject, messageToSend);
}

await consumerWrapper.StopAsync();

Console.WriteLine("Finished to subscribe to messages and will close...");