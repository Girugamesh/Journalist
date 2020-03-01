using System;
using System.Threading.Tasks;
using AutoFixture.AutoMoq;
using Journalist.Tasks;
using Journalist.WindowsAzure.Storage.Blobs;
using Moq;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class CloudBlockBlobContainerDataAttribute : AutoMoqDataAttribute
    {
        public CloudBlockBlobContainerDataAttribute(bool isExists = true, bool leaseLocked = false) : base(
            fixture =>
            {
                AutoFixture.FixtureRegistrar.Register(fixture, () => TimeSpan.FromMinutes(2));

                var failedTsc = new TaskCompletionSource<string>();
                failedTsc.SetException(new LeaseAlreadyAcquiredException());

                fixture.Customize<Mock<ICloudBlockBlob>>(composer => composer
                    .Do(mock => mock
                        .Setup(self => self.IsExistsAsync())
                        .Returns(isExists ? TaskDone.True : TaskDone.False))
                    .Do(mock => mock
                        .Setup(self => self.IsLeaseLocked())
                        .Returns(leaseLocked ? TaskDone.True : TaskDone.False))
                    .Do(mock => mock
                        .Setup(self => self.AcquireLeaseAsync(It.IsAny<TimeSpan?>(), null))
                        .Returns(leaseLocked ? failedTsc.Task : Task.FromResult("LeaseId")))
                    .Do(mock => mock
                        .Setup(self => self.Metadata)
                        .ReturnsUsingFixture(fixture))
                    .Do(mock => mock
                        .Setup(self => self.FetchAttributesAsync())
                        .Returns(TaskDone.Done))
                    .Do(mock => mock
                        .Setup(self => self.SaveMetadataAsync(It.IsAny<string>()))
                        .Returns(TaskDone.Done))
                    .Do(mock => mock
                        .Setup(self => self.BreakLeaseAsync(It.IsAny<TimeSpan?>()))
                        .Returns(TaskDone.Done))
                    .Do(mock => mock
                        .Setup(self => self.ReleaseLeaseAsync(It.IsAny<string>()))
                        .Returns(TaskDone.True)));

                fixture.Customize<Mock<ICloudBlobContainer>>(composer => composer
                    .Do(mock => mock
                        .Setup(self => self.CreateBlockBlob(It.IsAny<string>()))
                        .ReturnsUsingFixture(fixture)));

                return fixture;
            })
        {
        }
    }
}
