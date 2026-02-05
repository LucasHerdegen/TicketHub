using FluentAssertions;

namespace TicketHub.IntegrationTests.Tests
{
    public class ExampleTest : IClassFixture<TicketHubWebApplicationFactory>
    {
        private readonly TicketHubWebApplicationFactory _factory;

        public ExampleTest(TicketHubWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Test1()
        {
            true.Should().BeTrue();
        }
    }
}