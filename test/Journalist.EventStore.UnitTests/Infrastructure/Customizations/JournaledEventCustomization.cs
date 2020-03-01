using System.Collections.Generic;
using AutoFixture;
using Journalist.EventStore.Events;

namespace Journalist.EventStore.UnitTests.Infrastructure.Customizations
{
    public class JournaledEventCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JournaledEvent>(composer => composer
                .FromFactory(() => JournaledEvent.Create(
                    new object(),
                    (_, type, writer) => writer.WriteLine("EventPayload"))));

            fixture.Customize<IReadOnlyList<JournaledEvent>>(composer => composer
                .FromFactory((JournaledEvent[] events) => events));
        }
    }
}
