using Nats.Push.Subscriber.Console.NatsAbstraction;
using SystemConsole = System.Console;

namespace Nats.Push.Subscriber.console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SystemConsole.WriteLine("A small example of NATS push consumers!");

            string natsSubject = "my.nats.subject.example";
            string streamName = "my-stream";
            string durableName = "my-durable";

            NatsService natsPublishSubscribeService = new();
            natsPublishSubscribeService.CreateStream(streamName, [natsSubject]);
            natsPublishSubscribeService.CreateConsumer(streamName, durableName, natsSubject);

            static void newMessageReceivedHandler(object? sender, MessageHandlerEventArgs args)
            {
                SystemConsole.WriteLine($"Received message: {args.Data}");

                args.AcknowledgmentContext.Ack();
            }

            ISubscription subscription = natsPublishSubscribeService.SubscribeTopic(natsSubject, newMessageReceivedHandler, streamName, durableName);

            for (int i = 0; i < 100; i++)
            {
                string messageToSend = $"Message number {i}!";

                await natsPublishSubscribeService.PublishMessageAsync(natsSubject, messageToSend);
            }

            subscription.Unsubscribe();

            SystemConsole.WriteLine("Finished to subscribe to messages and will close...");
        }
    }
}
