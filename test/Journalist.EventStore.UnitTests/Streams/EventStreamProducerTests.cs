using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Journalist.EventStore.Events;
using Journalist.EventStore.Journal;
using Journalist.EventStore.Streams;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.UnitTests.Infrastructure.TestData;
using Moq;
using Xunit;

namespace Journalist.EventStore.UnitTests.Streams
{
    public class EventStreamProducerTests
    {
        [Theory]
        [EventStreamProducerDataCustomization]
        public async Task PublishAsync_AppendsEventsToStreamUsingStreamWriter(
            [Frozen] Mock<IEventStreamWriter> writerMock,
            JournaledEvent[] events,
            EventStreamProducer producer)
        {
            await producer.PublishAsync(events);

            writerMock.Verify(self => self.AppendEventsAsync(events));
        }

        [Theory]
        [EventStreamProducerDataCustomization]
        public async Task PublishAsync_WhenWriterThrows_MovesItToTheEndOfStreamAndTriesAppendAgain(
            [Frozen] Mock<IEventStreamWriter> writerMock,
            [Frozen] RetryPolicyStub retryPolicy,
            JournaledEvent[] events,
            EventStreamProducer producer)
        {
            var expectedAttemptsCount = 5;

            retryPolicy.ConfigureMaxAttemptNumber(expectedAttemptsCount);

            writerMock
                .Setup(self => self.AppendEventsAsync(events))
                .Throws<EventStreamConcurrencyException>();

            await Assert.ThrowsAsync<EventStreamConcurrencyException>(() => producer.PublishAsync(events));

            writerMock.Verify(self => self.MoveToEndOfStreamAsync());
            writerMock.Verify(self => self.AppendEventsAsync(events), Times.Exactly(expectedAttemptsCount));
        }
    }
}
