using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Journalist.EventStore.Notifications.Listeners;
using Journalist.EventStore.Notifications.Types;
using Journalist.EventStore.Streams;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.UnitTests.Infrastructure.TestData;
using Moq;
using Xunit;

namespace Journalist.EventStore.UnitTests.Notifications.Listeners
{
    public class EventConsumingNotificationListenerTests
    {
        [Theory, EventConsumingNotificationListenerData(throwException: true)]
        public async Task OnEventStreamUpdated_WhenProcessingFailed_DefersNotification(
            [Frozen] Mock<INotificationListenerSubscription> subscriptionMock,
            EventConsumingNotificationListenerStub listener,
            EventStreamUpdated notification)
        {
            await listener.On(notification);

            subscriptionMock.Verify(self => self.RetryNotificationProcessingAsync(notification), Times.Once());
        }

        [Theory, EventConsumingNotificationListenerData(throwException: true)]
        public async Task OnEventStreamUpdated_WhenProcessingFailed_CommitConsumedStreamVersion(
            [Frozen] Mock<IEventStreamConsumer> consumerMock,
            EventConsumingNotificationListenerStub listener,
            EventStreamUpdated notification)
        {
            await listener.On(notification);

            consumerMock.Verify(self => self.CommitProcessedStreamVersionAsync(true));
        }

        [Theory, EventConsumingNotificationListenerData]
        public async Task OnEventStreamUpdated_WhenProcessingCompleted_DoesNotDeferNotification(
            [Frozen] Mock<INotificationListenerSubscription> subscriptionMock,
            EventConsumingNotificationListenerStub listener,
            EventStreamUpdated notification)
        {
            await listener.On(notification);

            subscriptionMock.Verify(self => self.RetryNotificationProcessingAsync(notification), Times.Never());
        }
    }
}
