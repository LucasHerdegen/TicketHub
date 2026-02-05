using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketHub.API.Controllers;
using TicketHub.API.DTOs.Category;
using TicketHub.API.Models;
using TicketHub.API.Services.Interfaces;
using FluentValidation.Results;

namespace TicketHub.Tests.Controllers
{
    public class CategoryControllerTest
    {
        private readonly CategoryController _controller;
        private readonly Mock<ICategoryService> _mockService;
        private readonly Mock<IValidator<CategoryPostDto>> _mockPostValidator;
        private readonly Mock<IValidator<CategoryPutDto>> _mockPutValidator;

        public CategoryControllerTest()
        {
            _mockService = new Mock<ICategoryService>();
            _mockPostValidator = new Mock<IValidator<CategoryPostDto>>();
            _mockPutValidator = new Mock<IValidator<CategoryPutDto>>();

            _controller = new CategoryController(_mockService.Object, _mockPostValidator.Object, _mockPutValidator.Object);
        }

        [Fact]
        public async Task GetCategory_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange
            int id = -1;

            // act
            var result = await _controller.GetCategory(id);

            // assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetCategory_RetornaNotFound_SiNoExiste()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.GetCategory(id)).ReturnsAsync((CategoryDto?)null);

            // act
            var result = await _controller.GetCategory(id);

            // assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCategory_RetornaOk_SiExiste()
        {
            // arrange
            int id = 10;
            string name = "Deportivo";

            var category = new CategoryDto
            {
                Id = id,
                Name = name
            };

            _mockService.Setup(service => service.GetCategory(id)).ReturnsAsync(category);

            // act
            var result = await _controller.GetCategory(id);

            // assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = okResult.Value.Should().BeAssignableTo<CategoryDto>().Subject;
            dto.Id.Should().Be(id);
            dto.Name.Should().Be(name);
        }

        [Fact]
        public async Task CreateCategory_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            // arrange
            string name = "";

            var categoryPostDto = new CategoryPostDto
            {
                CategoryName = name
            };

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });

            _mockPostValidator.Setup(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act
            var result = await _controller.CreateCategory(categoryPostDto);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_RetornaConflict_SiLaCategoria_YaExiste()
        {
            // arrange
            string name = "Deportivo";

            var categoryPostDto = new CategoryPostDto
            {
                CategoryName = name
            };

            var validationResult = new ValidationResult();

            _mockPostValidator.Setup(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.CreateCategory(categoryPostDto)).ReturnsAsync((CategoryDto?)null);

            // act
            var result = await _controller.CreateCategory(categoryPostDto);

            // assert
            result.Should().BeOfType<ConflictResult>();
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateCategory(categoryPostDto), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_RetornaCreated_SiLaCategoria_NoExiste()
        {
            // arrange
            string name = "Deportivo";
            int id = 10;

            var categoryPostDto = new CategoryPostDto
            {
                CategoryName = name
            };
            var category = new CategoryDto
            {
                Id = id,
                Name = name
            };

            var validationResult = new ValidationResult();

            _mockPostValidator.Setup(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.CreateCategory(categoryPostDto)).ReturnsAsync(category);

            // act
            var result = await _controller.CreateCategory(categoryPostDto);

            // assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(categoryPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateCategory(categoryPostDto), Times.Once);

            createdResult.StatusCode.Should().Be(201);
            createdResult.Value.Should().Be(category);
            createdResult.ActionName.Should().Be(nameof(_controller.GetCategory));
            createdResult.RouteValues!["Id"].Should().Be(id);
        }

        [Fact]
        public async Task UpdateCategory_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            // arrange
            string name = "";
            int id = 10;

            var categoryPutDto = new CategoryPutDto
            {
                Name = name,
                Id = id
            };

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });

            _mockPutValidator.Setup(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act
            var result = await _controller.UpdateCategory(categoryPutDto);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_RetornaNotFound_SiLaCategoria_NoExiste()
        {
            // arrange
            string name = "Deportivo";
            int id = 10;

            var categoryPutDto = new CategoryPutDto
            {
                Name = name,
                Id = id
            };

            var validationResult = new ValidationResult();

            _mockPutValidator.Setup(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.UpdateCategory(categoryPutDto)).ReturnsAsync(false);

            // act
            var result = await _controller.UpdateCategory(categoryPutDto);

            // assert
            result.Should().BeOfType<NotFoundResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.UpdateCategory(categoryPutDto), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_RetornaNoContent_SiLaCategoriaExiste()
        {
            // arrange
            string name = "Deportivo";
            int id = 10;

            var categoryPutDto = new CategoryPutDto
            {
                Name = name,
                Id = id
            };

            var validationResult = new ValidationResult();

            _mockPutValidator.Setup(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.UpdateCategory(categoryPutDto)).ReturnsAsync(true);

            // act
            var result = await _controller.UpdateCategory(categoryPutDto);

            // assert
            result.Should().BeOfType<NoContentResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(categoryPutDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.UpdateCategory(categoryPutDto), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange
            int id = -1;

            // act
            var result = await _controller.DeleteCategory(id);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockService.Verify(service => service.DeleteCategory(id), Times.Never);
        }

        [Fact]
        public async Task DeleteCategory_RetornaNotFound_SiNoExiste()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.DeleteCategory(id)).ReturnsAsync(false);

            // act
            var result = await _controller.DeleteCategory(id);

            // assert
            result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(service => service.DeleteCategory(id), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_RetornaNoContent_SiExiste()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.DeleteCategory(id)).ReturnsAsync(true);

            // act
            var result = await _controller.DeleteCategory(id);

            // assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(service => service.DeleteCategory(id), Times.Once);
        }
    }
}