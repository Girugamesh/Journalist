using System;
using AutoFixture;
using Journalist.EventStore.Notifications.Listeners;
using Journalist.EventStore.UnitTests.Infrastructure.Customizations;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class EventConsumingNotificationListenerDataAttribute : AutoMoqDataAttribute
    {
        public EventConsumingNotificationListenerDataAttribute(bool throwException = false) :
            base(fixture =>
            {
                fixture.Customize(new EventStreamConsumerMoqCustomization(false));

                fixture.Customize<EventConsumingNotificationListenerStub>(composer => composer
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
