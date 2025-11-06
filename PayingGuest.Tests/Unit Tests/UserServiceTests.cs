using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace PayingGuest.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
        private readonly RegisterUserCommandHandler _handler;
        private readonly IIdentityService _identityService;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
            _handler = new RegisterUserCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _identityService);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnSuccess_WhenValidDataProvided()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                PropertyId = 1,
               // UserType = "Guest",
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@test.com",
                PhoneNumber = "+911234567890"
            };

            var command = new RegisterUserCommand { RegisterUserDto = registerDto };

            var property = new Property { PropertyId = 1, PropertyName = "Test Property" };
            var user = new User
            {
                UserId = 1,
                PropertyId = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@test.com",
                Property = property
            };

            var userDto = new UserDto
            {
                UserId = 1,
                PropertyId = 1,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@test.com",
                PropertyName = "Test Property"
            };

            _unitOfWorkMock.Setup(x => x.Users.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.Properties.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(property);

            _mapperMock.Setup(x => x.Map<User>(It.IsAny<RegisterUserDto>()))
                .Returns(user);

            _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
                .Returns(userDto);

            _unitOfWorkMock.Setup(x => x.Users.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.AuditLogs.AddAsync(It.IsAny<AuditLog>()))
                .ReturnsAsync(new AuditLog());

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("john.doe@test.com", result.Data.EmailAddress);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnError_WhenEmailExists()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                PropertyId = 1,
                //UserType = "Guest",
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "existing@test.com",
                PhoneNumber = "+911234567890"
            };

            var command = new RegisterUserCommand { RegisterUserDto = registerDto };

            _unitOfWorkMock.Setup(x => x.Users.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Email address already exists", result.Errors);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnError_WhenPropertyNotFound()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                PropertyId = 999,
                //UserType = "Guest",
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@test.com",
                PhoneNumber = "+911234567890"
            };

            var command = new RegisterUserCommand { RegisterUserDto = registerDto };

            _unitOfWorkMock.Setup(x => x.Users.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.Properties.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Property?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Property not found", result.Errors);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}