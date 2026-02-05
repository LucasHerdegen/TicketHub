using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using TicketHub.API.DTOs.Category;
using TicketHub.API.Models;
using TicketHub.API.Repository;
using TicketHub.API.Services.Implementations;

namespace TicketHub.Tests.Services
{
    public class CategoryServiceTest
    {
        private readonly CategoryService _service;
        private readonly Mock<IRepository<Category>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;

        public CategoryServiceTest()
        {
            _mockRepo = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();

            _service = new CategoryService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCategory_RetornaNull_SiLaCategoria_NoExiste()
        {
            // arrange
            int id = 10;
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync((Category?)null);

            // act
            var result = await _service.GetCategory(id);

            // assert
            result.Should().BeNull();
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task GetCategory_RetornaLaCategoria_SiLaCategoriaExiste()
        {
            // arrange
            int id = 10;
            string name = "Deportivo";

            var category = new Category
            {
                Id = id,
                Name = name
            };
            var categoryDto = new CategoryDto
            {
                Id = id,
                Name = name
            };

            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(category);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDto>(category)).Returns(categoryDto);


            // act
            var result = await _service.GetCategory(id);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(categoryDto);
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<CategoryDto>(category), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_RetornaNull_SiLaCategoriaExiste()
        {
            // arrange
            string name = "Deportivo";

            var categoryPostDto = new CategoryPostDto
            {
                Name = name,
            };

            _mockRepo.Setup(repo => repo.Any(c => c.Name!.ToUpper() == categoryPostDto.Name!.ToUpper()))
                .ReturnsAsync(true);

            // act
            var result = await _service.CreateCategory(categoryPostDto);

            // assert
            result.Should().BeNull();
            _mockRepo.Verify(repo =>
                repo.Any(c => c.Name!.ToUpper() == categoryPostDto.Name!.ToUpper()), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task CreateCategory_RetornaLaCategoria_SiLaCategoria_NoExiste()
        {
            // arrange
            string name = "Deportivo";
            int id = 10;

            var categoryPostDto = new CategoryPostDto
            {
                Name = name,
            };
            var categoryStored = new Category
            {
                Id = id,
                Name = name
            };
            var categoryDto = new CategoryDto
            {
                Id = id,
                Name = name
            };

            _mockRepo.Setup(repo => repo.Any(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(false);
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Category>()))
                .Callback<Category>(c => c.Id = id);
            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(categoryStored);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDto>(It.IsAny<Category>())).Returns(categoryDto);

            // act
            var result = await _service.CreateCategory(categoryPostDto);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(categoryDto);
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Category>()), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_RetornaFalse_SiNoExiste()
        {
            // arrange
            int id = 10;
            string name = "Deportivo";

            var categoryPutDto = new CategoryPutDto
            {
                Id = id,
                Name = name
            };

            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync((Category?)null);

            // act
            var result = await _service.UpdateCategory(categoryPutDto);

            // assert
            result.Should().BeFalse();
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task UpdateCategory_RetornaTrue_SiExiste()
        {
            // arrange
            int id = 10;
            string name = "Deportivo";

            var categoryPutDto = new CategoryPutDto
            {
                Id = id,
                Name = name
            };
            var category = new Category
            {
                Id = id,
                Name = name
            };

            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(category);

            // act
            var result = await _service.UpdateCategory(categoryPutDto);

            // assert
            result.Should().BeTrue();
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_RetornaFalse_SiNoExiste()
        {
            // arrange
            int id = 10;

            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync((Category?)null);

            // act
            var result = await _service.DeleteCategory(id);

            // assert
            result.Should().BeFalse();
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task DeleteCategory_RetornaFalse_SiExiste()
        {
            // arrange
            int id = 10;
            string name = "Deportivo";

            var category = new Category
            {
                Id = id,
                Name = name
            };

            _mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(category);

            // act
            var result = await _service.DeleteCategory(id);

            // assert
            result.Should().BeTrue();
            _mockRepo.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepo.Verify(repo => repo.Save(), Times.Once);
        }
    }
}