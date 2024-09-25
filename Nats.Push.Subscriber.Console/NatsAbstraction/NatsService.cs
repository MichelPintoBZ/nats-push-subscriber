using NATS.Client;
using NATS.Client.JetStream;
using System.Text;
using static NATS.Client.JetStream.PushSubscribeOptions;

namespace Nats.Push.Subscriber.Console.NatsAbstraction
{
    internal class NatsService
    {
        private const string NatsServerUrl = "nats://nats-server:4222";

        private readonly IConnection _connection;

        public NatsService()
        {
            Options natsOptions = ConnectionFactory.GetDefaultOptions();
            natsOptions.Servers = [NatsServerUrl];

            _connection = new ConnectionFactory().CreateConnection(natsOptions);
        }

        public void CreateStream(string streamName, string[] topics)
        {
            IJetStreamManagement jetStreamManagement = _connection.CreateJetStreamManagementContext();

            StreamConfiguration streamConfiguration = StreamConfiguration.Builder()
                .WithName(streamName)
                .WithSubjects(topics)
                .WithStorageType(StorageType.Memory)
                .Build();

            _ = jetStreamManagement.AddStream(streamConfiguration);
        }

        public void CreateConsumer(string streamName, string durableName, string filterSubject)
        {
            IJetStreamManagement jetStreamManagement = _connection.CreateJetStreamManagementContext();

            ConsumerConfiguration consumerConfiguration = ConsumerConfiguration.Builder()
                .WithDurable(durableName)
                .WithDeliverSubject(durableName)
                .WithDeliverGroup(durableName)
                .WithName(durableName)
                .WithFilterSubject(filterSubject)
                .Build();

            _ = jetStreamManagement.AddOrUpdateConsumer(streamName, consumerConfiguration);
        }

        public ISubscription SubscribeTopic(string subjectName, EventHandler<MessageHandlerEventArgs> handler, string streamName, string durableName)
        {
            IJetStreamPushAsyncSubscription subscription = default!;

            try
            {
                string queueName = durableName;

                IJetStreamManagement jetStreamManagement = _connection.CreateJetStreamManagementContext();
                StreamInfo streamInfo = GetStreamInfoOrNullWhenNotExist(jetStreamManagement, streamName);
                if (streamInfo is null)
                {
                    return default!;
                }

                void PrivateEventHandler(object? sender, MsgHandlerEventArgs args)
                {
                    string data = Encoding.UTF8.GetString(args.Message.Data);
                    string subject = args.Message.Subject;
                    MessageHandlerEventArgs messageHandlerEventArgs = new(data, subject, new NatsAcknowledgmentContext(args.Message));

                    handler.Invoke(sender, messageHandlerEventArgs);
                }

                PushSubscribeOptions pso = BindTo(streamName, durableName);

                IJetStream jetStream = _connection.CreateJetStreamContext();

                subscription = jetStream.PushSubscribeAsync(subjectName, queueName, PrivateEventHandler, false, pso);
            }
            catch (NATSConnectionException)
            {
                return default!;
            }
            catch (NATSJetStreamClientException)
            {
                return default!;
            }

            return new Subscription(subscription);
        }

        public async Task PublishMessageAsync(string subjectName, string message)
        {
            byte[] encodedMessageToSend = Encoding.UTF8.GetBytes(message);

            IJetStream jetStream = _connection.CreateJetStreamContext();
            _ = await jetStream.PublishAsync(subjectName, encodedMessageToSend).ConfigureAwait(false);
        }

        private static StreamInfo GetStreamInfoOrNullWhenNotExist(IJetStreamManagement jetStreamManagement, string streamName)
        {
            ArgumentNullException.ThrowIfNull(jetStreamManagement);
            ArgumentException.ThrowIfNullOrEmpty(streamName);

            try
            {
                return jetStreamManagement.GetStreamInfo(streamName);
            }
            catch (NATSJetStreamException)
            {
                return default!;
            }
        }
    }
}
