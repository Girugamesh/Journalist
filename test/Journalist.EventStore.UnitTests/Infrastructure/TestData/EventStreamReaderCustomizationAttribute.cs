using System;
using System.Threading.Tasks;
using AutoFixture;
using Journalist.EventStore.Events;
using Journalist.EventStore.Streams;
using Journalist.EventStore.UnitTests.Infrastructure.Customizations;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class EventStreamReaderCustomizationAttribute : AutoMoqDataAttribute
    {
        public EventStreamReaderCustomizationAttribute(
            bool hasEvents = true,
            bool completed = false,
            bool leaderPromotion = true,
            bool disableAutoCommit = false) : base(
            fixture =>
            {
                fixture.Customize(new EventStreamReaderCustomization(completed, hasEvents));
                fixture.Customize(new EventStreamConsumingSessionCustomization(leaderPromotion));
                fixture.Customize(new CommitStreamVersionFMockCustomization());

                fixture.Customize<EventStreamConsumer>(composer => composer.FromFactory(
                    () => new EventStreamConsumer(
                        fixture.Create<IEventStreamConsumingSession>(),
                        fixture.Create<IEventStreamReaderFactory>(),
                        !disableAutoCommit,
                        fixture.Create<Func<StreamVersion, Task>>())));
                return fixture;
            })
        {
        }
    }
}
