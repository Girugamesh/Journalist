using System;
using System.Collections.Generic;
using System.Linq;
using Journalist.Collections;
using Journalist.EventStore.Notifications;
using Journalist.EventStore.Notifications.Types;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.Utils.Polling;
using Journalist.WindowsAzure.Storage.Blobs;
using Moq;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class PendingNotificationsChaserDataAttribute : AutoMoqDataAttribute
    {
        public PendingNotificationsChaserDataAttribute(bool lockWasNotAcquired = false, bool noNotifications = false) : base(
            fixture =>
            {
                fixture.Customize<IPollingJob>(composer => composer.FromFactory((PollingJobStub stub) => stub));
                fixture.Customize<PendingNotificationsChaser>(composer => composer.Do(chaser => chaser.Start()));

                if (lockWasNotAcquired)
                {
                    fixture.Customize<Mock<ICloudBlockBlob>>(composer => composer
                        .Do(mock => mock
                            .Setup(self => self.AcquireLeaseAsync(It.IsAny<TimeSpan?>(), null))
                            .Throws<LeaseAlreadyAcquiredException>()));
                }

                if (noNotifications)
                {
                    fixture.Customize<IDictionary<string, List<EventStreamUpdated>>>(composer => composer
                        .FromFactory(() => new Dictionary<string, List<EventStreamUpdated>>()));
                }
                else
                {
                    fixture.Customize<IDictionary<string, List<EventStreamUpdated>>>(composer => composer
                        .FromFactory((EventStreamUpdated[] notifications) => notifications.ToDictionary(
                            n => n.StreamName,
                            n => n.YieldList())));
                }
                return fixture;
            })
        {
        }
    }
}
