using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Models;

namespace TicketHub.IntegrationTests.Tests
{
    public class TicketTest : IClassFixture<TicketHubWebApplicationFactory>
    {
        private readonly TicketHubWebApplicationFactory _factory;
        private readonly string _userId = "1";
        public TicketTest(TicketHubWebApplicationFactory factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task GetTicket_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync($"/api/ticket/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTicket_RetornaNotFound_SiElTicket_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync($"/api/ticket/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetTicket_RetornaOk_SiElTicketExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;
            int categoryId = 1;
            int eventId = 1;

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = categoryId,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = eventId,
                    CategoryId = categoryId
                });
                await context.SaveChangesAsync();

                await context.Tickets.AddAsync(new Ticket
                {
                    EventId = eventId,
                    Id = id,
                    UserId = _userId
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.GetAsync($"/api/ticket/{id}");
            var result = await response.Content.ReadFromJsonAsync<TicketDto>();

            // assert
            var okResult = response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task CreateTicket_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int eventId = -1;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PostAsync("/api/ticket", JsonContent.Create(ticketPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTicket_RetornaConflict_SiElEvento_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int eventId = 1;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PostAsync("/api/ticket", JsonContent.Create(ticketPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateTicket_RetornaConflict_SiElEvento_EstaAgotado()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int eventId = 1;
            int categoryId = 1;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                await context!.Categories.AddAsync(new Category
                {
                    Id = categoryId,
                    Name = "Rock"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = eventId,
                    CategoryId = categoryId,
                    Capacity = 1,
                    Name = "Evento Lleno",
                    Date = DateTime.Now.AddDays(10),
                    Price = 100,
                    SoldTickets = 1
                });
                await context.SaveChangesAsync();

                await context.Tickets.AddAsync(new Ticket
                {
                    EventId = eventId,
                    UserId = "otro-usuario-id"
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.PostAsync("/api/ticket", JsonContent.Create(ticketPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateTicket_RetornaCreated_SiElEventoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int eventId = 1;
            int categoryId = 1;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = categoryId,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = eventId,
                    CategoryId = categoryId,
                    Capacity = 10
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.PostAsync("/api/ticket", JsonContent.Create(ticketPostDto));
            var result = await response.Content.ReadFromJsonAsync<TicketDto>();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result!.EventId.Should().Be(eventId);
        }

        [Fact]
        public async Task DeleteTicket_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/ticket/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTicket_RetornaNotFound_SiElTicket_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/ticket/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteTicket_RetornaNoContent_SiElTicketExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;
            int eventId = 1;
            int categoryId = 1;

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = categoryId,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = eventId,
                    CategoryId = categoryId
                });
                await context.SaveChangesAsync();

                await context.Tickets.AddAsync(new Ticket
                {
                    EventId = eventId,
                    Id = id,
                    UserId = _userId
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.DeleteAsync($"/api/ticket/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        private HttpClient CreateAuthenticatedClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Test");

            return client;
        }
    }
}