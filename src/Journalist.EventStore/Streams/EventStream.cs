using System.Threading.Tasks;
using Journalist.EventStore.Journal;
using Journalist.EventStore.Streams.Serializers;

namespace Journalist.EventStore.Streams
{
    public class EventStream : IEventStream
    {
        private readonly IEventJournal m_journal;
        private readonly IEventSerializer m_serializer;

        public EventStream(IEventJournal journal, IEventSerializer serializer)
        {
            Require.NotNull(journal, "journal");
            Require.NotNull(serializer, "serializer");

            m_journal = journal;
            m_serializer = serializer;
        }

        public async Task<IEventStreamReader> OpenReaderAsync(string streamName)
        {
            Require.NotEmpty(streamName, "streamName");

            var reader = new EventStreamReader(
                streamName,
                await m_journal.OpenEventStreamAsync(streamName),
                m_serializer);

            return reader;
        }

        public async Task<IEventStreamReader> OpenReaderAsync(string streamName, int streamVersion)
        {
            Require.NotEmpty(streamName, "streamName");
            Require.Positive(streamVersion, "streamVersion");

            var reader = new EventStreamReader(
                streamName,
                await m_journal.OpenEventStreamAsync(streamName, StreamVersion.Create(streamVersion)),
                m_serializer);

            return reader;
        }

        public async Task<IEventStreamWriter> OpenWriterAsync(string streamName)
        {
            Require.NotEmpty(streamName, "streamName");

            var endOfStream = await m_journal.ReadEndOfStreamPositionAsync(streamName);

            return new EventStreamWriter(
                streamName,
                endOfStream,
                m_journal,
                m_serializer);;
        }
    }
}
