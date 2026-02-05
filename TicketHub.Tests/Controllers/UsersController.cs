using TicketHub.API.Controllers;
using TicketHub.API.DTOs.User;
using TicketHub.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TicketHub.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk_WhenCalled()
        {
            _userServiceMock.Setup(service => service.GetUsers())
                .ReturnsAsync(new List<UserDto>());

            var result = await _controller.GetUsers();

            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeAssignableTo<IEnumerable<UserDto>>();
        }
    }
}