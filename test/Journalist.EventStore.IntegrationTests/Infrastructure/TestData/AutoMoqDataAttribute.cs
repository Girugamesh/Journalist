using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Journalist.EventStore.IntegrationTests.Infrastructure.Customizations.Customizations;

namespace Journalist.EventStore.IntegrationTests.Infrastructure.TestData
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization { ConfigureMembers = true })
                .Customize(new JournaledEventCustomization()))
        {
        }
    }
}
