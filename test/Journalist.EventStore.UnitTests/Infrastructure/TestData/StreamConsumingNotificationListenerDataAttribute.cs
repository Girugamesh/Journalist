using AutoFixture;
using Journalist.EventStore.Notifications.Listeners;
using Journalist.EventStore.UnitTests.Infrastructure.Customizations;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class StreamConsumingNotificationListenerDataAttribute : AutoMoqDataAttribute
    {
        public StreamConsumingNotificationListenerDataAttribute(
            bool startedSubscription = true,
            bool processingFailed = false,
            bool consumerReceivingFailed = false) : base(
            fixture =>
            {
                fixture.Customize(new EventStreamConsumerMoqCustomization(consumerReceivingFailed));
                fixture.Customize(new EventStreamConsumerMoqCustomization(consumerReceivingFailed));

                fixture.Customize<StreamConsumingNotificationListenerStub>(composer => composer
                    .Do(stub =>
                    {
                        if (startedSubscription)
                        {
                            stub.OnSubscriptionStarted(fixture.Create<INotificationListenerSubscription>());
                        }

                        stub.ProcessingCompleted = !processingFailed;
                    })
                    .OmitAutoProperties());
                return fixture;
            })
        {
        }
    }
}
