using AutoFixture.AutoMoq;
using Journalist.EventStore.Notifications;
using Journalist.EventStore.Notifications.Listeners;
using Moq;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class NotificationListenerSubscriptionDataAttribute : AutoMoqDataAttribute
    {
        public NotificationListenerSubscriptionDataAttribute() : base(
            fixture =>
            {
                fixture.Customize<Mock<INotification>>(composer => composer
                    .Do(mock => mock
                        .Setup(self => self.SendTo(It.IsAny<INotificationListener>()))
                        .ReturnsUsingFixture(fixture))
                    .Do(mock => mock
                        .SetupGet(self => self.DeliveryCount)
                        .Returns(0)));
                return fixture;
            })
        {
        }
    }
}
