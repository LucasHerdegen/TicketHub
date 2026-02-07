using System.Net;
using FluentAssertions;

namespace TicketHub.IntegrationTests.Tests
{
    public class CategoryTest : IClassFixture<TicketHubWebApplicationFactory>
    {
        private readonly TicketHubWebApplicationFactory _factory;

        public CategoryTest(TicketHubWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetCategory_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var result = await client.GetAsync($"/api/category/{id}");

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCategory_RetornaNotFound_SiLaCategoria_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var result = await client.GetAsync($"/api/category/{id}");

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

            return client;
        }
    }
}