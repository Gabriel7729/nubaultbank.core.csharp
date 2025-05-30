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
        private readonly DefaultHttpContext _httpContext;
        private readonly Create _sut;

        public CreateTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _mapperMock = new Mock<IMapper>();
            _logServiceMock = new Mock<ILogService>();
            _sut = new Create(_userRepositoryMock.Object, _mapperMock.Object, _logServiceMock.Object);
            _httpContext = new DefaultHttpContext();
            _sut.ControllerContext = new ControllerContext { HttpContext = _httpContext };
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsSuccessAndLogs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString(), Name = "Test Name" };
            var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
            var beneficiary = new Beneficiary { Name = "Test Name" };
            var beneficiaryDto = new BeneficiaryDTO { Name = "Test Name" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<Beneficiary>(requestDto)).Returns(beneficiary);
            _mapperMock.Setup(m => m.Map<BeneficiaryDTO>(beneficiary)).Returns(beneficiaryDto);
            _userRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _logServiceMock.Setup(l => l.CreateLog(_httpContext, It.IsAny<string>(), ActionStatus.Success, userId, null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var resultValue = okResult.Value as Result<BeneficiaryDTO>;
            resultValue.IsSuccess.Should().BeTrue();
            resultValue.Value.Should().BeEquivalentTo(beneficiaryDto);
            resultValue.SuccessMessage.Should().Be("Beneficiario agregado con éxito");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<Beneficiary>(requestDto), Times.Once);
            _mapperMock.Verify(m => m.Map<BeneficiaryDTO>(beneficiary), Times.Once);
            _logServiceMock.Verify(l => l.CreateLog(_httpContext, "Beneficiario agregado con éxito", ActionStatus.Success, userId, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_InvalidUserIdFormat_ReturnsBadRequestAndNoLog()
        {
            // Arrange
            var requestDto = new BeneficiaryDTO { UserId = "invalid-guid" };

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var resultValue = badRequest.Value as Result<BeneficiaryResponseDTO>;
            resultValue.IsSuccess.Should().BeFalse();
            resultValue.Errors.Should().Contain("El formato del UserId no es válido");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ActionStatus>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_UserNotFound_ReturnsBadRequestAndNoLog()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var resultValue = badRequest.Value as Result<BeneficiaryResponseDTO>;
            resultValue.IsSuccess.Should().BeFalse();
            resultValue.Errors.Should().Contain("El usuario no fue encontrado");
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _logServiceMock.Verify(l => l.CreateLog(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ActionStatus>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_RepositoryThrowsException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            var exception = new Exception("Repository error");
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ThrowsAsync(exception);
            _logServiceMock.Setup(l => l.CreateLog(_httpContext, It.IsAny<string>(), ActionStatus.Error, null, It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var resultValue = badRequest.Value as Result<BeneficiaryResponseDTO>;
            resultValue.IsSuccess.Should().BeFalse();
            resultValue.Errors.Should().Contain("Ha ocurrido un error al agregar el beneficiario");
            resultValue.Errors.Should().Contain("Repository error");
            _logServiceMock.Verify(l => l.CreateLog(_httpContext, "Ha ocurrido un error al agregar el beneficiario", ActionStatus.Error, null, It.Is<string>(msg => msg.Contains("Repository error")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_MapperThrowsException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString() };
            var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
            var exception = new Exception("Mapping error");
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<Beneficiary>(requestDto)).Throws(exception);
            _logServiceMock.Setup(l => l.CreateLog(_httpContext, It.IsAny<string>(), ActionStatus.Error, null, It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var resultValue = badRequest.Value as Result<BeneficiaryResponseDTO>;
            resultValue.IsSuccess.Should().BeFalse();
            resultValue.Errors.Should().Contain("Ha ocurrido un error al agregar el beneficiario");
            resultValue.Errors.Should().Contain("Mapping error");
            _logServiceMock.Verify(l => l.CreateLog(_httpContext, "Ha ocurrido un error al agregar el beneficiario", ActionStatus.Error, null, It.Is<string>(msg => msg.Contains("Mapping error")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_UpdateAsyncThrowsException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestDto = new BeneficiaryDTO { UserId = userId.ToString(), Name = "Test Name" };
            var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
            var beneficiary = new Beneficiary { Name = "Test Name" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<Beneficiary>(requestDto)).Returns(beneficiary);
            _userRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Update error"));
            _logServiceMock.Setup(l => l.CreateLog(_httpContext, It.IsAny<string>(), ActionStatus.Error, null, It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.HandleAsync(requestDto);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            var resultValue = badRequest.Value as Result<BeneficiaryResponseDTO>;
            resultValue.IsSuccess.Should().BeFalse();
            resultValue.Errors.Should().Contain("Ha ocurrido un error al agregar el beneficiario");
            resultValue.Errors.Should().Contain("Update error");
            _logServiceMock.Verify(l => l.CreateLog(_httpContext, "Ha ocurrido un error al agregar el beneficiario", ActionStatus.Error, null, It.Is<string>(msg => msg.Contains("Update error")), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 