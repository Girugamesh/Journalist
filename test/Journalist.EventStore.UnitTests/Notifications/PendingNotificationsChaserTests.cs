using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Journalist.EventStore.Events;
using Journalist.EventStore.Notifications;
using Journalist.EventStore.Notifications.Types;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.UnitTests.Infrastructure.TestData;
using Journalist.WindowsAzure.Storage.Blobs;
using Moq;
using Xunit;

namespace Journalist.EventStore.UnitTests.Notifications
{
    public class PendingNotificationsChaserTests
    {
        [Theory, PendingNotificationsChaserData(lockWasNotAcquired: true)]
        public async Task OnPoll_WhenLockWasNotAcquired_ReturnsFalse(
            [Frozen] PollingJobStub jobStub)
        {
            var pollResult = await jobStub.Poll();

            Assert.False(pollResult);
        }

        [Theory, PendingNotificationsChaserData(lockWasNotAcquired: true)]
        public async Task OnPoll_WhenLockWasNotAcquired_DoesNotLoadPendingNotifications(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<IPendingNotifications> pendingNotificationsMock)
        {
            await jobStub.Poll();

            pendingNotificationsMock.Verify(self => self.LoadAsync(), Times.Never());
        }

        [Theory, PendingNotificationsChaserData]
        public async Task OnPoll_WhenLockWasAcquired_LoadsPendingNotifications(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<IPendingNotifications> pendingNotificationsMock)
        {
            await jobStub.Poll();

            pendingNotificationsMock.Verify(self => self.LoadAsync());
        }

        [Theory, PendingNotificationsChaserData(noNotifications: true)]
        public async Task OnPoll_WhenPendingNotificationListIsEmpty_ReturnsFalse(
            [Frozen] PollingJobStub jobStub)
        {
            var pollResult = await jobStub.Poll();

            Assert.False(pollResult);
        }

        [Theory, PendingNotificationsChaserData]
        public async Task OnPoll_WhenPendingNotificationListIsNotEmpty_ReturnsTrue(
            [Frozen] PollingJobStub jobStub)
        {
            var pollResult = await jobStub.Poll();

            Assert.True(pollResult);
        }

        [Theory, PendingNotificationsChaserData()]
        public async Task OnPoll_WhenPendingNotificationListIsNotEmpty_SendNotificationToTheHub(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<INotificationHub> hubMock,
            [Frozen] EventStreamUpdated[] pendingNotifications)
        {
            await jobStub.Poll();

            foreach (var pendingNotification in pendingNotifications)
            {
                hubMock.Verify(self => self.NotifyAsync(pendingNotification));
            }
        }

        [Theory, PendingNotificationsChaserData]
        public async Task OnPoll_WhenPendingNotificationListIsNotEmpty_DeletesNotification(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<IPendingNotifications> pendingNotificationsMock,
            [Frozen] EventStreamUpdated[] pendingNotifications)
        {
            await jobStub.Poll();


            foreach (var pendingNotification in pendingNotifications)
            {
                pendingNotificationsMock.Verify(self => self.DeleteAsync(
                    pendingNotification.StreamName,
                    It.Is<StreamVersion[]>(versions => versions.Contains(pendingNotification.FromVersion))));
            }
        }

        [Theory, PendingNotificationsChaserData]
        public async Task OnPoll_WhenPendingNotificationListIsNotEmpty_ReleasesAcquiredLock(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<ICloudBlockBlob> blobLock)
        {
            await jobStub.Poll();

            blobLock.Verify(self => self.ReleaseLeaseAsync(It.IsAny<string>()));
        }

        [Theory, PendingNotificationsChaserData()]
        public async Task OnPoll_WhenExceptionHappensOnProcessing_ReleasesAcquiredLock(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<IPendingNotifications> pendingNotificationsMock,
            [Frozen] Mock<ICloudBlockBlob> blobLock)
        {
            pendingNotificationsMock
                .Setup(self => self.DeleteAsync(It.IsAny<string>(), It.IsAny<StreamVersion>()))
                .Throws<Exception>();

            await jobStub.Poll();

            blobLock.Verify(self => self.ReleaseLeaseAsync(It.IsAny<string>()));
        }

        [Theory, PendingNotificationsChaserData()]
        public async Task OnPoll_WhenExceptionHappensOnProcessing_ReturnsFalse(
            [Frozen] PollingJobStub jobStub,
            [Frozen] Mock<IPendingNotifications> pendingNotificationsMock)
        {
            pendingNotificationsMock
                .Setup(self => self.DeleteAsync(It.IsAny<string>(), It.IsAny<StreamVersion[]>()))
                .Throws<Exception>();

            var pollResult = await jobStub.Poll();

            Assert.False(pollResult);
        }
    }
}
