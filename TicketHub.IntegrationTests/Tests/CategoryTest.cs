using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TicketHub.API.DTOs.Category;
using TicketHub.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

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
            var response = await client.GetAsync($"/api/category/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCategory_RetornaNotFound_SiLaCategoria_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync($"/api/category/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCategory_RetornaOk_SiLaCategoriaExiste()
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
                    Id = id,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.GetAsync($"/api/category/{id}");
            var result = await response.Content.ReadFromJsonAsync<CategoryDto>();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task CreateCategory_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "";

            var categoryPostDto = new CategoryPostDto
            {
                Name = name
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PostAsync($"/api/category", JsonContent.Create(categoryPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCategory_RetornaConflict_SiLaCategoria_YaExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "Deportivo";

            var categoryPostDto = new CategoryPostDto
            {
                Name = name
            };

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = 1,
                    Name = name
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.PostAsync($"/api/category", JsonContent.Create(categoryPostDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateCategory_RetornaCreated_SiLaCategoria_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "Deportivo";

            var categoryPostDto = new CategoryPostDto
            {
                Name = name
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PostAsync($"/api/category", JsonContent.Create(categoryPostDto));
            var headers = response.Headers;

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            headers.Contains("location").Should().BeTrue();
        }

        [Fact]
        public async Task UpdateCategory_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "";
            int id = -1;

            var categoryPutDto = new CategoryPutDto
            {
                Id = id,
                Name = name
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PutAsync($"/api/category", JsonContent.Create(categoryPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateCategory_RetornaNotFound_SiLaCategoria_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "Deportivo";
            int id = 1;

            var categoryPutDto = new CategoryPutDto
            {
                Id = id,
                Name = name
            };

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.PutAsync($"/api/category", JsonContent.Create(categoryPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCategory_RetornaNoContent_SiLaCategoriaExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            string name = "Deportivo";
            int id = 1;

            var categoryPutDto = new CategoryPutDto
            {
                Id = id,
                Name = name
            };

            var client = CreateAuthenticatedClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();

                if (context == null)
                    throw new Exception("Something went wrong finding the context...");

                await context.Categories.AddAsync(new Category
                {
                    Id = id,
                    Name = "Otro nombre"
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.PutAsync($"/api/category", JsonContent.Create(categoryPutDto));

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteCategory_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = -1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/category/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteCategory_RetornaNotFound_SiLaCategoria_NoExiste()
        {
            await _factory.EnsureCleanDatabaseAsync();

            // arrange
            int id = 1;

            var client = CreateAuthenticatedClient();

            // act
            var response = await client.DeleteAsync($"/api/category/{id}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCategory_RetornaNoContent_SiLaCategoriaExiste()
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
                    Id = id,
                    Name = "Deportivo"
                });
                await context.SaveChangesAsync();
            }

            // act
            var response = await client.DeleteAsync($"/api/category/{id}");

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