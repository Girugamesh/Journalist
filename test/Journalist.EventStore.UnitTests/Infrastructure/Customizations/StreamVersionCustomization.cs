using AutoFixture;
using Journalist.EventStore.Events;

namespace Journalist.EventStore.UnitTests.Infrastructure.Customizations
{
    public class StreamVersionCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<StreamVersion>(composer => composer.FromFactory(
                (byte version) => StreamVersion.Create(version)));
        }
    }
}
