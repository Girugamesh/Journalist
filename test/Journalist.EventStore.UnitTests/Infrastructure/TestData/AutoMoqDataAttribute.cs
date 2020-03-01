using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Journalist.EventStore.UnitTests.Infrastructure.Customizations;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() =>
                new Fixture()
                    .Customize(new AutoMoqCustomization
                    {
                        ConfigureMembers = true
                    })
                    .Customize(new JournaledEventCustomization())
                    .Customize(new IOStreamCustomization()))
        {
        }

        public AutoMoqDataAttribute(Func<IFixture, IFixture> combinedFixtureFactory)
            : base(() =>
            {
                if (combinedFixtureFactory == null)
                {
                    throw new ArgumentNullException(nameof(combinedFixtureFactory));
                }

                var fixture = new Fixture()
                    .Customize(new AutoMoqCustomization
                    {
                        ConfigureMembers = true
                    })
                    .Customize(new JournaledEventCustomization())
                    .Customize(new IOStreamCustomization());

                return combinedFixtureFactory(fixture);
            })
        {
        }
    }
}
