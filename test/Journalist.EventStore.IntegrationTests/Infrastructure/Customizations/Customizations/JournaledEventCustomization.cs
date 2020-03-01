using System;
using AutoFixture;
using Journalist.EventStore.Events;

namespace Journalist.EventStore.IntegrationTests.Infrastructure.Customizations.Customizations
{
    public class JournaledEventCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JournaledEvent>(composer => composer
                .FromFactory((Guid headerName, string headerValue) =>
                {
                    var result = JournaledEvent.Create(
                        new object(),
                        (_, type, writer) => writer.WriteLine("EventPayload"));

                    result.SetHeader(headerName.ToString("N"), headerValue);

                    return result;
                }));
        }
    }
}
