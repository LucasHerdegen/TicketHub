using System.Security.Claims;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketHub.API.Controllers;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.Tests.Controllers
{
    public class TicketControllerTest
    {
        // ... tus mocks normales ...
        private readonly TicketController _controller;
        private readonly Mock<ITicketService> _mockService;
        private readonly Mock<IValidator<TicketPostDto>> _mockValidator;
        private readonly string _userId = "test-user-id-123";

        public TicketControllerTest()
        {
            _mockService = new Mock<ITicketService>();
            _mockValidator = new Mock<IValidator<TicketPostDto>>();

            _controller = new TicketController(_mockService.Object, _mockValidator.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }


        [Fact]
        public async Task GetTicket_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange
            int id = -1;

            // act
            var result = await _controller.GetTicket(id);

            // assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _mockService.Verify(service => service.GetTicket(id, _userId), Times.Never);
        }

        [Fact]
        public async Task GetTicket_RetornaNotFound_SiElTicket_NoExiste()
        {
            // arrange
            int id = 10;

            _mockService.Setup(service => service.GetTicket(id, _userId)).ReturnsAsync((TicketDto?)null);

            // act
            var result = await _controller.GetTicket(id);

            // assert
            result.Result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(service => service.GetTicket(id, _userId), Times.Once);
        }

        [Fact]
        public async Task GetTicket_RetornaOk_SiElTicketExiste()
        {
            // arrange
            int id = 10;

            var ticket = new TicketDto
            {
                Id = id,
                UserId = _userId
            };

            _mockService.Setup(service => service.GetTicket(id, _userId)).ReturnsAsync(ticket);

            // act
            var result = await _controller.GetTicket(id);

            // assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = okResult.Value.Should().BeAssignableTo<TicketDto>().Subject;
            dto.Should().Be(ticket);
            _mockService.Verify(service => service.GetTicket(id, _userId), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_RetornaBadRequest_SiLaValidacion_NoPasa()
        {
            // arrange
            var ticketPostDto = new TicketPostDto
            {
                EventId = -1
            };

            var validationResult = new ValidationResult(new[]
                { new ValidationFailure("Event", "Event id must be greater than zero") });

            _mockValidator.Setup(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // act
            var result = await _controller.CreateTicket(ticketPostDto);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockValidator
                .Verify(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_RetornaConflict_SiElServicio_NoPuedeCrearlo()
        {
            // arrange
            var ticketPostDto = new TicketPostDto
            {
                EventId = 1
            };

            var validationResult = new ValidationResult();

            _mockValidator.Setup(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.CreateTicket(ticketPostDto, _userId)).ReturnsAsync((TicketDto?)null);

            // act
            var result = await _controller.CreateTicket(ticketPostDto);

            // assert
            result.Should().BeOfType<ConflictResult>();
            _mockValidator
                .Verify(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateTicket(ticketPostDto, _userId), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_RetornaCreated_SiElServicio_PuedeCrearlo()
        {
            // arrange
            int eventId = 1;
            int id = 10;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };
            var ticketDto = new TicketDto
            {
                EventId = eventId,
                UserId = _userId,
                Id = id
            };

            var validationResult = new ValidationResult();

            _mockValidator.Setup(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            _mockService.Setup(service => service.CreateTicket(ticketPostDto, _userId)).ReturnsAsync(ticketDto);

            // act
            var result = await _controller.CreateTicket(ticketPostDto);

            // assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            _mockValidator
                .Verify(validator => validator.ValidateAsync(ticketPostDto, It.IsAny<CancellationToken>()), Times.Once);
            _mockService.Verify(service => service.CreateTicket(ticketPostDto, _userId), Times.Once);

            createdResult.StatusCode.Should().Be(201);
            createdResult.Value.Should().Be(ticketDto);
            createdResult.ActionName.Should().Be(nameof(_controller.GetTicket));
            createdResult.RouteValues!["Id"].Should().Be(id);
        }

        [Fact]
        public async Task DeleteTicket_RetornaBadRequest_SiElId_NoEsPositivo()
        {
            // arrange
            int id = -1;

            // act
            var result = await _controller.DeleteTicket(id);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteTicket_RetornaNotFound_SiElServicio_NoLoEncuentra()
        {
            // arrange
            int id = 1;

            _mockService.Setup(service => service.DeleteTicket(id, _userId)).ReturnsAsync(false);

            // act
            var result = await _controller.DeleteTicket(id);

            // assert
            result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(service => service.DeleteTicket(id, _userId), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_RetornaNoContent_SiElServicio_PuedeEliminarlo()
        {
            // arrange
            int id = 1;

            _mockService.Setup(service => service.DeleteTicket(id, _userId)).ReturnsAsync(true);

            // act
            var result = await _controller.DeleteTicket(id);

            // assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(service => service.DeleteTicket(id, _userId), Times.Once);
        }
    }
}