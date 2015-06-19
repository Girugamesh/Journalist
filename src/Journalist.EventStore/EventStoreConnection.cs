using System.Threading.Tasks;
using Journalist.EventStore.Journal;
using Journalist.EventStore.Streams;
using Journalist.EventStore.Utils;

namespace Journalist.EventStore
{
    public class EventStoreConnection : IEventStoreConnection
    {
        private readonly IEventJournal m_journal;

        public EventStoreConnection(IEventJournal journal)
        {
            Require.NotNull(journal, "journal");

            m_journal = journal;
        }

        public async Task<IEventStreamReader> CreateStreamReaderAsync(string streamName)
        {
            Require.NotEmpty(streamName, "streamName");

            var reader = new EventStreamReader(
                streamName,
                await m_journal.OpenEventStreamAsync(streamName));

            return reader;
        }

        public async Task<IEventStreamReader> CreateStreamReaderAsync(string streamName, int streamVersion)
        {
            Require.NotEmpty(streamName, "streamName");
            Require.Positive(streamVersion, "streamVersion");

            var reader = new EventStreamReader(
                streamName,
                await m_journal.OpenEventStreamAsync(streamName, StreamVersion.Create(streamVersion)));

            return reader;
        }

        public async Task<IEventStreamWriter> CreateStreamWriterAsync(string streamName)
        {
            Require.NotEmpty(streamName, "streamName");

            var endOfStream = await m_journal.ReadEndOfStreamPositionAsync(streamName);

            return new EventStreamWriter(
                streamName,
                endOfStream,
                m_journal);
        }

        public async Task<IEventStreamProducer> CreateStreamProducer(string streamName)
        {
            return new EventStreamProducer(await CreateStreamWriterAsync(streamName), RetryPolicy.Default);
        }

        public async Task<IEventStreamConsumer> CreateStreamConsumer(string streamName)
        {
            return new EventStreamConsumer();
        }
    }
}
