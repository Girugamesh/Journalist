using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using Journalist.Collections;
using Journalist.EventStore.Connection;
using Journalist.EventStore.Notifications;
using Journalist.EventStore.Notifications.Channels;
using Journalist.EventStore.Notifications.Formatters;
using Journalist.EventStore.Notifications.Listeners;
using Journalist.EventStore.Notifications.Processing;
using Journalist.EventStore.Notifications.Types;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.Utils.Polling;
using Journalist.Tasks;
using Moq;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class NotificationHubDataAttribute : AutoMoqDataAttribute
    {
        public NotificationHubDataAttribute(
            bool emptyChannel = false,
            bool hasSubscriber = true,
            bool startHub = true) : base(
            fixture =>
            {
                fixture.Customize<INotification>(composer => composer.FromFactory((EventStreamUpdated notification) => notification));

                fixture.Customize<IReceivedNotification[]>(composer => composer.FromFactory((IReceivedNotification n) => n.YieldArray()));

                fixture.Customize<Mock<IReceivedNotificationProcessor>>(composer =>
                    composer.Do(mock => mock.Setup(self => self.ProcessingCount).Returns(0)));

                fixture.Customize<Mock<INotificationsChannel>>(composer => composer
                    .Do(mock => mock.Setup(self => self.ReceiveNotificationsAsync())
                        .Returns(() => emptyChannel
                            ? EmptyArray.Get<IReceivedNotification>().YieldTask()
                            : fixture.Create<IReceivedNotification[]>().YieldTask()))
                    .Do(mock => mock.Setup(self => self.SendAsync(It.IsAny<INotification>())).Returns(TaskDone.Done)));

                fixture.Customize<IPollingJob>(composer => composer.FromFactory((PollingJobStub stub) => stub));

                fixture.Customize<Mock<INotificationFormatter>>(composer => composer
                    .Do(mock => mock.Setup(self => self.FromBytes(It.IsAny<Stream>())).Returns(fixture.Create<EventStreamUpdated>))
                    .Do(mock => mock.Setup(self => self.ToBytes(It.IsAny<EventStreamUpdated>())).ReturnsUsingFixture(fixture)));

                fixture.Customize<NotificationHub>(composer => composer.Do(hub =>
                {
                    if (hasSubscriber)
                    {
                        hub.Subscribe(fixture.Create<INotificationListener>());
                    }

                    if (startHub)
                    {
                        hub.StartNotificationProcessing(fixture.Create<IEventStoreConnection>());
                    }
                }));
                return fixture;
            })
        {
        }
    }
}
