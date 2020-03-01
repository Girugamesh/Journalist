using Journalist.EventStore.UnitTests.Infrastructure.Stubs;
using Journalist.EventStore.Utils.RetryPolicies;

namespace Journalist.EventStore.UnitTests.Infrastructure.TestData
{
    public class EventStreamProducerDataCustomizationAttribute : AutoMoqDataAttribute
    {
        public EventStreamProducerDataCustomizationAttribute() :
            base(fixture =>
            {
                fixture.Customize<IRetryPolicy>(composer => composer
                    .FromFactory((RetryPolicyStub stub) => stub));
                return fixture;
            })
        {
        }
    }
}
