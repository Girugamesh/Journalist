using System;
using AutoFixture;
using Journalist.EventStore.Notifications.Listeners;
using Journalist.EventStore.UnitTests.Infrastructure.Customizations;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class BatchEventConsumingNotificationListenerDataAttribute : AutoMoqDataAttribute
    {
        public BatchEventConsumingNotificationListenerDataAttribute(bool throwException = false) :
            base(fixture =>
            {
                fixture.Customize(new EventStreamConsumerMoqCustomization(false));

                fixture.Customize<BatchEventConsumingNotificationListenerStub>(composer => composer
                    .Do(stub =>
                    {
                        stub.OnSubscriptionStarted(fixture.Create<INotificationListenerSubscription>());

                        if (throwException)
                        {
                            stub.Exception = fixture.Create<Exception>();
                        }

                    }).OmitAutoProperties());
                return fixture;
            })
        {
        }
    }
}
