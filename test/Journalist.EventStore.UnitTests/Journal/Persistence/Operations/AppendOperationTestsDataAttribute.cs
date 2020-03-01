using Journalist.EventStore.Events;
using Journalist.EventStore.Journal;
using Journalist.EventStore.UnitTests.Infrastructure.TestData;

namespace Journalist.EventStore.UnitTests.Journal.Persistence.Operations
{
    public class AppendOperationTestsDataAttribute : AutoMoqDataAttribute
    {
        public AppendOperationTestsDataAttribute(bool isNewStream = false) : base(
            fixture =>
            {
                fixture.Customize<EventStreamHeader>(composer => composer
                    .FromFactory((string etag, StreamVersion version) =>
                        isNewStream
                            ? EventStreamHeader.Unknown
                            : new EventStreamHeader(etag, version)));
                return fixture;
            })
        {
        }
    }
}
