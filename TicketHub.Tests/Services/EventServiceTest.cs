using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using TicketHub.API.DTOs.Event;
using TicketHub.API.Models;
using TicketHub.API.Repository;
using TicketHub.API.Services.Implementations;

namespace TicketHub.Tests.Services
{
    public class EventServiceTest
    {
        private readonly EventService _service;
        private readonly Mock<IEventRepository> _mockRepository;
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;

        public EventServiceTest()
        {
            _mockRepository = new Mock<IEventRepository>();
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();

            _service = new EventService(_mockRepository.Object, _mockMapper.Object, _mockCategoryRepository.Object);
        }

        [Fact]
        public async Task GetEvent_RetornaNull_SiElEvento_NoExiste()
        {
            // arrange
            int id = 10;

            _mockRepository.Setup(repo => repo.GetEventByIdWithCategory(id)).ReturnsAsync((Event?)null);

            // act
            var result = await _service.GetEvent(id);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetEventByIdWithCategory(id), Times.Once);
        }

        [Fact]
        public async Task GetEvent_RetornaElEvento_SiExiste()
        {
            // arrange
            int id = 10;

            var evnt = new Event();
            var eventDto = new EventDto();

            _mockRepository.Setup(repo => repo.GetEventByIdWithCategory(id)).ReturnsAsync(evnt);
            _mockMapper.Setup(mapper => mapper.Map<EventDto>(evnt)).Returns(eventDto);

            // act
            var result = await _service.GetEvent(id);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(eventDto);
            _mockRepository.Verify(repo => repo.GetEventByIdWithCategory(id), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<EventDto>(evnt), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaNull_SiYaExiste()
        {
            // arrange
            var eventPostDto = new EventPostDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Name = "Deportivo",
                Price = 20
            };

            _mockRepository.Setup(repo => repo.Any(EventExpression))
                .ReturnsAsync(true);

            // act
            var result = await _service.CreateEvent(eventPostDto);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaNull_SiLaCategoria_NoExiste()
        {
            // arrange
            var eventPostDto = new EventPostDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Name = "Deportivo",
                Price = 20
            };

            _mockRepository.Setup(repo => repo.Any(EventExpression))
                .ReturnsAsync(false);
            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(false);

            // act
            var result = await _service.CreateEvent(eventPostDto);

            // assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
        }

        [Fact]
        public async Task CreateEvent_RetornaElEvento_SiElEventoNoExiste_YLaCategoriaSi()
        {
            // arrange
            int id = 10;
            var eventPostDto = new EventPostDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Name = "Deportivo",
                Price = 20
            };
            var evnt = new Event
            {
                Capacity = eventPostDto.Capacity,
                CategoryId = eventPostDto.CategoryId,
                Date = eventPostDto.Date,
                Name = eventPostDto.Name,
                Price = eventPostDto.Price
            };
            var eventDto = new EventDto
            {
                Capacity = eventPostDto.Capacity,
                CategoryId = eventPostDto.CategoryId,
                Date = eventPostDto.Date,
                Name = eventPostDto.Name,
                Price = eventPostDto.Price,
                Id = id
            };

            _mockRepository.Setup(repo => repo.Any(EventExpression))
                .ReturnsAsync(false);
            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(true);
            _mockMapper.Setup(mapper => mapper.Map<Event>(eventPostDto)).Returns(evnt);
            _mockRepository.Setup(repo => repo.Create(It.IsAny<Event>()))
                .Callback<Event>(c => c.Id = id);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(evnt);
            _mockMapper.Setup(mapper => mapper.Map<EventDto>(evnt)).Returns(eventDto);

            // act
            var result = await _service.CreateEvent(eventPostDto);

            // assert
            result.Should().NotBeNull();
            result.Should().Be(eventDto);
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaFalse_SiLaCategoria_NoExiste()
        {
            // arrange
            int id = 10;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Id = id,
                Name = "Deportivo",
                Price = 10
            };

            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(false);

            // act
            var result = await _service.UpdateEvent(eventPutDto);

            // assert
            result.Should().BeFalse();
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaFalse_SiHayOtroEvento_ConEsasCaracteristicas()
        {
            // arrange
            int id = 10;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Id = id,
                Name = "Deportivo",
                Price = 10
            };

            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(true);
            _mockRepository.Setup(repo => repo.Any(EventExpression)).ReturnsAsync(true);

            // act
            var result = await _service.UpdateEvent(eventPutDto);

            // assert
            result.Should().BeFalse();
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaFalse_SiElEvento_NoExiste()
        {
            // arrange
            int id = 10;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Id = id,
                Name = "Deportivo",
                Price = 10
            };

            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(true);
            _mockRepository.Setup(repo => repo.Any(EventExpression)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Event?)null);

            // act
            var result = await _service.UpdateEvent(eventPutDto);

            // assert
            result.Should().BeFalse();
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_RetornaTrue_SiElEvento_CumpleLosRequisitos()
        {
            // arrange
            int id = 10;

            var eventPutDto = new EventPutDto
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Id = id,
                Name = "Deportivo",
                Price = 10
            };
            var evnt = new Event
            {
                Capacity = 10,
                CategoryId = 1,
                Date = DateTime.Now.AddDays(3),
                Id = id,
                Name = "Deportivo",
                Price = 15
            };

            _mockCategoryRepository.Setup(repo => repo.Any(CategoryExpression)).ReturnsAsync(true);
            _mockRepository.Setup(repo => repo.Any(EventExpression)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(evnt);

            // act
            var result = await _service.UpdateEvent(eventPutDto);

            // assert
            result.Should().BeTrue();
            _mockCategoryRepository.Verify(repo => repo.Any(CategoryExpression), Times.Once);
            _mockRepository.Verify(repo => repo.Any(EventExpression), Times.Once);
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_RetornaFalse_SiElEvento_NoExiste()
        {
            // arrange
            int id = 10;

            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Event?)null);

            // act
            var result = await _service.DeleteEvent(id);

            // assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_RetornaTrue_SiElEventoExiste()
        {
            // arrange
            int id = 10;

            var evnt = new Event
            {
                Id = id
            };

            _mockRepository.Setup(repo => repo.GetById(id)).ReturnsAsync(evnt);

            // act
            var result = await _service.DeleteEvent(id);

            // assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.GetById(id), Times.Once);
            _mockRepository.Verify(repo => repo.Delete(evnt), Times.Once);
        }


        private Expression<Func<Event, bool>> EventExpression => It.IsAny<Expression<Func<Event, bool>>>();
        private Expression<Func<Category, bool>> CategoryExpression => It.IsAny<Expression<Func<Category, bool>>>();
    }
}