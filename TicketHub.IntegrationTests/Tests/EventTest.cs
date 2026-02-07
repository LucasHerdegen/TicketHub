using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TicketHub.API.DTOs.Category;
using TicketHub.API.DTOs.Event;
using TicketHub.API.Models;

namespace TicketHub.IntegrationTests.Tests
{
    public class EventTest : IClassFixture<TicketHubWebApplicationFactory>
    {
        private readonly TicketHubWebApplicationFactory _factory;

        public EventTest(TicketHubWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetEvent_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync($"/api/event/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetEvent_RetornaNotFound_SiElEvento_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync($"/api/event/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetEvent_RetornaOk_SiElEventoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;
            int categoryId = 1;

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = 1,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = id,
                    CategoryId = categoryId
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.GetAsync($"/api/event/{id}");
            var result = await response.Content.ReadFromJsonAsync<EventDto>();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task CreateEvent_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            var eventPostDto = new EventPostDto
            {
                Capacity = -1,
                CategoryId = -1,
                Date = DateTime.Now,
                Name = "",
                Price = -1
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PostAsync($"/api/event", JsonContent.Create(eventPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_RetornaConflict_SiElEvento_YaExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            var date = DateTime.Now.AddDays(1);
            string name = "Deportivo";
            int categoryId = 1;

            var eventPostDto = new EventPostDto
            {
                Capacity = 10,
                CategoryId = categoryId,
                Date = date,
                Name = name,
                Price = 100
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
                    Id = 1,
                    CategoryId = categoryId,
                    Date = date,
                    Name = name
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.PostAsync($"/api/event", JsonContent.Create(eventPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateEvent_RetornaCreated_SiElEvento_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            var date = DateTime.Now.AddDays(1);
            string name = "Deportivo";
            int categoryId = 1;

            var eventPostDto = new EventPostDto
            {
                Capacity = 10,
                CategoryId = categoryId,
                Date = date,
                Name = name,
                Price = 100
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
            }

            // act
            var response = await client.PostAsync($"/api/event", JsonContent.Create(eventPostDto));
            var result = await response.Content.ReadFromJsonAsync<EventDto>();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result!.CategoryId.Should().Be(categoryId);
        }

        [Fact]
        public async Task UpdateEvent_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            var eventPutDto = new EventPutDto
            {
                Capacity = -1,
                CategoryId = -1,
                Date = DateTime.Now,
                Id = -1,
                Name = "",
                Price = -1
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PutAsync($"/api/event", JsonContent.Create(eventPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEvent_RetornaConflict_SiElEvento_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int categoryId = 1;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = categoryId,
                Date = DateTime.Now.AddDays(4),
                Id = 1,
                Name = "Deportes",
                Price = 100
            };

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
            }

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PutAsync($"/api/event", JsonContent.Create(eventPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task UpdateEvent_RetornaNoContent_SiElEventoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;
            int categoryId = 1;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = categoryId,
                Date = DateTime.Now.AddDays(4),
                Id = id,
                Name = "Deportes",
                Price = 100
            };

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
                    Id = id
                });
                await context.SaveChangesAsync();
            }

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PutAsync($"/api/event", JsonContent.Create(eventPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteEvent_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/event/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteEvent_RetornaNotFound_SiElEvento_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/event/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteEvent_RetornaNoContent_SiElEventoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = 1,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();

                await context.Events.AddAsync(new Event
                {
                    Id = id
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.DeleteAsync($"/api/event/{id}");

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