using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Models;
using TicketHub.API.Repository;
using TicketHub.API.Services.Implementations;

namespace TicketHub.Tests.Services
{
    public class TicketServiceTest
    {
        private readonly TicketService _service;
        private readonly Mock<ITicketRepository> _mockRepository;
        private readonly Mock<IRepository<Event>> _mockEventRepository;
        private readonly Mock<IMapper> _mockMapper;

        public TicketServiceTest()
        {
            _mockRepository = new Mock<ITicketRepository>();
            _mockEventRepository = new Mock<IRepository<Event>>();
            _mockMapper = new Mock<IMapper>();

            _service = new TicketService(_mockRepository.Object, _mockMapper.Object, _mockEventRepository.Object);
        }

        [Fact]
        public async Task GetTicket_RetornaNull_SiElTicket_NoExiste()
        {
            // arrange
            int id = 10;
            string userId = "test@gmail.com";

            _mockRepository.Setup(repo => repo.GetTicketWithEvent(id, userId)).ReturnsAsync((Ticket?)null);

            // act
            var result = await _service.GetTicket(id, userId);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetTicketWithEvent(id, userId), Times.Once);
        }

        [Fact]
        public async Task GetTicket_RetornaElTicket_SiElTicketExiste()
        {
            // arrange
            int id = 10;
            string userId = "test@gmail.com";

            var ticket = new Ticket
            {
                Id = id,
                UserId = userId
            };
            var ticketDto = new TicketDto
            {
                Id = id,
                UserId = userId
            };

            _mockRepository.Setup(repo => repo.GetTicketWithEvent(id, userId)).ReturnsAsync(ticket);
            _mockMapper.Setup(mapper => mapper.Map<TicketDto>(ticket)).Returns(ticketDto);

            // act
            var result = await _service.GetTicket(id, userId);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(ticketDto);
            _mockRepository.Verify(repo => repo.GetTicketWithEvent(id, userId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<TicketDto>(ticket), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_RetornaNull_SiYaExiste()
        {
            // arrange
            int eventId = 10;
            string userId = "prueba@gmail.com";

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            _mockRepository.Setup(repo => repo.Any(TicketExpression)).ReturnsAsync(true);

            // act
            var result = await _service.CreateTicket(ticketPostDto, userId);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.Any(TicketExpression), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetById(eventId), Times.Never);
        }

        [Fact]
        public async Task CreateTicket_RetornaNull_SiElEvento_NoExiste()
        {
            // arrange
            int eventId = 10;
            string userId = "prueba@gmail.com";

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };

            _mockRepository.Setup(repo => repo.Any(TicketExpression)).ReturnsAsync(false);
            _mockEventRepository.Setup(repo => repo.GetById(eventId)).ReturnsAsync((Event?)null);

            // act
            var result = await _service.CreateTicket(ticketPostDto, userId);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.Any(TicketExpression), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetById(eventId), Times.Once);
            _mockRepository.Verify(repo => repo.Count(TicketExpression), Times.Never);
        }

        [Fact]
        public async Task CreateTicket_RetornaNull_SiElEvento_EstaLleno()
        {
            // arrange
            int eventId = 10;
            string userId = "prueba@gmail.com";
            int capacity = 10;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };
            var evnt = new Event
            {
                Id = eventId,
                Capacity = capacity
            };

            _mockRepository.Setup(repo => repo.Any(TicketExpression)).ReturnsAsync(false);
            _mockEventRepository.Setup(repo => repo.GetById(eventId)).ReturnsAsync(evnt);
            _mockRepository.Setup(repo => repo.Count(TicketExpression)).ReturnsAsync(capacity);

            // act
            var result = await _service.CreateTicket(ticketPostDto, userId);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.Any(TicketExpression), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetById(eventId), Times.Once);
            _mockRepository.Verify(repo => repo.Count(TicketExpression), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_RetornaElTicket_SiElTicket_NoExiste_Y_ElEvento_NoEstaLleno()
        {
            // arrange
            int id = 1;
            int eventId = 10;
            string userId = "prueba@gmail.com";
            int capacity = 10;

            var ticketPostDto = new TicketPostDto
            {
                EventId = eventId
            };
            var evnt = new Event
            {
                Id = eventId,
                Capacity = capacity
            };
            var ticket = new Ticket
            {
                EventId = eventId,
                Id = id,
                Event = evnt,
                UserId = userId
            };
            var ticketDto = new TicketDto
            {
                EventId = eventId,
                Id = id,
                UserId = userId
            };

            _mockRepository.Setup(repo => repo.Any(TicketExpression)).ReturnsAsync(false);
            _mockEventRepository.Setup(repo => repo.GetById(eventId)).ReturnsAsync(evnt);
            _mockRepository.Setup(repo => repo.Count(TicketExpression)).ReturnsAsync(0);
            _mockRepository.Setup(repo => repo.Create(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => t.Id = id);
            _mockRepository.Setup(repo => repo.GetTicketWithEvent(id, userId)).ReturnsAsync(ticket);
            _mockMapper.Setup(mapper => mapper.Map<TicketDto>(ticket)).Returns(ticketDto);

            // act
            var result = await _service.CreateTicket(ticketPostDto, userId);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(ticketDto);
            _mockRepository.Verify(repo => repo.Any(TicketExpression), Times.Once);
            _mockEventRepository.Verify(repo => repo.GetById(eventId), Times.Once);
            _mockRepository.Verify(repo => repo.Count(TicketExpression), Times.Once);
            _mockRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_RetornaFalse_SiElTicket_NoExiste()
        {
            // arrange
            int id = 10;
            string userId = "prueba@gmail.com";

            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Ticket?)null);

            // act
            var result = await _service.DeleteTicket(id, userId);

            // assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_RetornaFalse_SiElTicket_NoEsDelUsuario()
        {
            // arrange
            int id = 10;
            string userId = "prueba@gmail.com";
            string userId2 = "test@gmail.com";

            var ticket = new Ticket
            {
                Id = id,
                UserId = userId2
            };

            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(ticket);

            // act
            var result = await _service.DeleteTicket(id, userId);

            // assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_RetornaTrue_SiElTicket_EsDelUsuario()
        {
            // arrange
            int id = 10;
            string userId = "prueba@gmail.com";

            var ticket = new Ticket
            {
                Id = id,
                UserId = userId
            };

            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(ticket);

            // act
            var result = await _service.DeleteTicket(id, userId);

            // assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepository.Verify(repo => repo.Save(), Times.Once);
        }

        private Expression<Func<Event, bool>> EventExpression => It.IsAny<Expression<Func<Event, bool>>>();
        private Expression<Func<Ticket, bool>> TicketExpression => It.IsAny<Expression<Func<Ticket, bool>>>();
    }
}