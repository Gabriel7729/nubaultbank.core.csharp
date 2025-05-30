using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.SharedKernel.Interfaces;
using NuBaultBank.Web.Endpoints.BeneficiaryEndpoints;
using Xunit;

namespace NuBaultBank.UnitTest.Handlers
{
    public class CreateTests
    {
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly Create _handler;

        public CreateTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _mapperMock = new Mock<IMapper>();
            _logServiceMock = new Mock<ILogService>();
            _handler = new Create(_userRepositoryMock.Object, _mapperMock.Object, _logServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsSuccessResultAndLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
            var beneficiary = new Beneficiary();
            var beneficiaryDto = new BeneficiaryDTO();

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<Beneficiary>(requestDto)).Returns(beneficiary);
            _mapperMock.Setup(m => m.Map<BeneficiaryDTO>(beneficiary)).Returns(beneficiaryDto);
            _userRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _logServiceMock.Setup(l => l.CreateLog(It.IsAny<HttpContext>(), It.IsAny<string>(), ActionStatus.Success, userId, null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.HandleAsync(requestDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var value = okResult.Value as Result<BeneficiaryDTO>;
            value.IsSuccess.Should().BeTrue();
            value.Value.Should().Be(beneficiaryDto);
            value.Errors.Should().BeNullOrEmpty();
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<Beneficiary>(requestDto), Times.Once);
            _mapperMock.Verify(m => m.Map<BeneficiaryDTO>(beneficiary), Times.Once);
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.Is<string>(s => s.Contains("éxito")), ActionStatus.Success, userId, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_InvalidUserId_ReturnsBadRequestAndDoesNotLog()
        {
            // Arrange
            var requestDto = new BeneficiaryDTO { UserId = "invalid-guid" };

            // Act
            var result = await _handler.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var value = badRequest.Value as Result<BeneficiaryResponseDTO>;
            value.IsSuccess.Should().BeFalse();
            value.Errors.Should().ContainSingle(e => e.Contains("UserId"));
            _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ActionStatus>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_UserNotFound_ReturnsBadRequestAndLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act
            var result = await _handler.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var value = badRequest.Value as Result<BeneficiaryResponseDTO>;
            value.IsSuccess.Should().BeFalse();
            value.Errors.Should().ContainSingle(e => e.Contains("usuario"));
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ActionStatus>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_RepositoryThrowsException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("db error"));

            // Act
            var result = await _handler.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var value = badRequest.Value as Result<BeneficiaryResponseDTO>;
            value.IsSuccess.Should().BeFalse();
            value.Errors.Should().Contain(e => e.Contains("error"));
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.Is<string>(s => s.Contains("error")), ActionStatus.Error, null, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_MapperThrowsException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<Beneficiary>(requestDto)).Throws(new Exception("mapping error"));

            // Act
            var result = await _handler.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var value = badRequest.Value as Result<BeneficiaryResponseDTO>;
            value.IsSuccess.Should().BeFalse();
            value.Errors.Should().Contain(e => e.Contains("error"));
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.Is<string>(s => s.Contains("error")), ActionStatus.Error, null, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 