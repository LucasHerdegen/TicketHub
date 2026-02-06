using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketHub.API.Controllers;
using TicketHub.API.DTOs.Event;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.Tests.Controllers
{
    public class EventControllerTest
    {
        private readonly EventController _controller;
        private readonly Mock<IEventService> _mockService;
        private readonly Mock<IValidator<EventPostDto>> _mockPostValidator;
        private readonly Mock<IValidator<EventPutDto>> _mockPutValidator;

        public EventControllerTest()
        {
            _mockService = new Mock<IEventService>();
            _mockPostValidator = new Mock<IValidator<EventPostDto>>();
            _mockPutValidator = new Mock<IValidator<EventPutDto>>();

            _controller = new EventController(_mockService.Object, _mockPostValidator.Object, _mockPutValidator.Object);
        }

        [Fact]
        public async Task GetEvent_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange}
            int id = -1;

            // act
            var result = await _controller.GetEvent(id);

            // assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _mockService.Verify(service => service.GetEvent(id), Times.Never);
        }

        [Fact]
        public async Task GetEvent_RetornaNotFound_SiElEvento_NoExiste()
        {
            // arrange}
            int id = 10;

            _mockService.Setup(service => service.GetEvent(id)).ReturnsAsync((EventDto?)null);

            // act
            var result = await _controller.GetEvent(id);

            // assert
            result.Result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(service => service.GetEvent(id), Times.Once);
        }

        [Fact]
        public async Task GetEvent_RetornaOk_SiElEventoExiste()
        {
            // arrange}
            int id = 10;

            var evnt = new EventDto
            {
                Id = id
            };

            _mockService.Setup(service => service.GetEvent(id)).ReturnsAsync(evnt);

            // act
            var result = await _controller.GetEvent(id);

            // assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = okResult.Value.Should().BeAssignableTo<EventDto>().Subject;
            dto.Should().Be(evnt);
            _mockService.Verify(service => service.GetEvent(id), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            // arrange
            var eventPostDto = new EventPostDto
            {
                Name = "",
                Capacity = -1,
                CategoryId = -1,
                Date = DateTime.Now,
                Price = -1
            };

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });

            _mockPostValidator.Setup(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act
            var result = await _controller.CreateEvent(eventPostDto);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaConflict_SiElServicio_NoPuedeCrearlo()
        {
            // arrange
            var eventPostDto = new EventPostDto
            {
                Name = "Deportivo",
                Capacity = 100,
                CategoryId = 1,
                Date = DateTime.Now,
                Price = 20
            };

            var validationResult = new ValidationResult();

            _mockPostValidator.Setup(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockService.Setup(service => service.CreateEvent(eventPostDto)).ReturnsAsync((EventDto?)null);

            // act
            var result = await _controller.CreateEvent(eventPostDto);

            // assert
            result.Should().BeOfType<ConflictResult>();
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateEvent(eventPostDto), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaCreated_SiElServicio_PuedeCrearlo()
        {
            // arrange
            int id = 10;

            var eventPostDto = new EventPostDto
            {
                Name = "Deportivo",
                Capacity = 100,
                CategoryId = 1,
                Date = DateTime.Now,
                Price = 20
            };
            var eventDto = new EventDto
            {
                Name = "Deportivo",
                Capacity = 100,
                CategoryId = 1,
                Date = DateTime.Now,
                Price = 20,
                Id = id
            };

            var validationResult = new ValidationResult();

            _mockPostValidator.Setup(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockService.Setup(service => service.CreateEvent(eventPostDto)).ReturnsAsync(eventDto);

            // act
            var result = await _controller.CreateEvent(eventPostDto);

            // assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            _mockPostValidator
                .Verify(validator => validator.ValidateAsync(eventPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateEvent(eventPostDto), Times.Once);

            createdResult.StatusCode.Should().Be(201);
            createdResult.Value.Should().Be(eventDto);
            createdResult.ActionName.Should().Be(nameof(_controller.GetEvent));
            createdResult.RouteValues!["Id"].Should().Be(id);
        }

        [Fact]
        public async Task UpdateEvent_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
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

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });

            _mockPutValidator.Setup(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act
            var result = await _controller.UpdateEvent(eventPutDto);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaConflict_SiElServicio_NoPuedeActualizarlo()
        {
            // arrange
            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(4),
                Id = 1,
                Name = "Deportivo",
                Price = 100
            };

            var validationResult = new ValidationResult();

            _mockPutValidator.Setup(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.UpdateEvent(eventPutDto)).ReturnsAsync(false);

            // act
            var result = await _controller.UpdateEvent(eventPutDto);

            result.Should().BeOfType<ConflictResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.UpdateEvent(eventPutDto), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaNoContent_SiElServicio_PuedeActualizarlo()
        {
            // arrange
            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(4),
                Id = 1,
                Name = "Deportivo",
                Price = 100
            };

            var validationResult = new ValidationResult();

            _mockPutValidator.Setup(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.UpdateEvent(eventPutDto)).ReturnsAsync(true);

            // act
            var result = await _controller.UpdateEvent(eventPutDto);

            result.Should().BeOfType<NoContentResult>();
            _mockPutValidator
                .Verify(validator => validator.ValidateAsync(eventPutDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.UpdateEvent(eventPutDto), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange
            int id = -1;

            // act
            var result = await _controller.DeleteEvent(id);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteEvent_RetornaNotFound_SiElServicio_NoPuedeEliminarlo()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.DeleteEvent(id)).ReturnsAsync(false);

            // act
            var result = await _controller.DeleteEvent(id);

            // assert
            result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(service => service.DeleteEvent(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_RetornaNoContent_SiElServicio_PuedeEliminarlo()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.DeleteEvent(id)).ReturnsAsync(true);

            // act
            var result = await _controller.DeleteEvent(id);

            // assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(service => service.DeleteEvent(id), Times.Once);
        }
    }
}