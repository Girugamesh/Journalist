using System;
using System.Threading.Tasks;
using AutoFixture;
using Journalist.EventStore.Events;
using Journalist.EventStore.UnitTests.Infrastructure.Stubs;

namespace Journalist.EventStore.UnitTests.Infrastructure.Customizations
{
    public class CommitStreamVersionFMockCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Func<StreamVersion, Task>>(composer => composer
                .FromFactory((CommitStreamVersionFMock mock) => mock.Invoke));
        }
    }
}
