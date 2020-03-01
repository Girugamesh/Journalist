using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.UnitTests.Infrastructure.TestData;

namespace Journalist.EventStore.UnitTests.Notifications.Processing
{
    public class ReceivedNotificationProcessorTestsDataAttribute : AutoMoqDataAttribute
    {
        public ReceivedNotificationProcessorTestsDataAttribute(bool throwOnNotificationHandling = false) : base(
            fixture =>
            {
                fixture.Customize<NotificationHandlerStub>(composer => composer
                    .FromFactory(() => new NotificationHandlerStub(throwOnNotificationHandling)));
                return fixture;
            })
        {
        }
    }
}
