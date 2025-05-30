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

namespace NuBaultBank.UnitTest.Endpoints.BeneficiaryEndpoints;

public class CreateTests
{
    private readonly Mock<IRepository<User>> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogService> _mockLogService;
    private readonly Create _handler;
    private readonly Mock<HttpContext> _mockHttpContext;

    public CreateTests()
    {
        _mockUserRepository = new Mock<IRepository<User>>();
        _mockMapper = new Mock<IMapper>();
        _mockLogService = new Mock<ILogService>();
        _mockHttpContext = new Mock<HttpContext>();
        _handler = new Create(_mockUserRepository.Object, _mockMapper.Object, _mockLogService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            }
        };
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryDto = new BeneficiaryDTO { UserId = userId.ToString() };
        var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
        var beneficiary = new Beneficiary();
        var mappedBeneficiaryDto = new BeneficiaryDTO();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<Beneficiary>(beneficiaryDto))
            .Returns(beneficiary);
        _mockMapper.Setup(m => m.Map<BeneficiaryDTO>(beneficiary))
            .Returns(mappedBeneficiaryDto);
        _mockUserRepository.Setup(r => r.UpdateAsync(user, default))
            .Returns(Task.CompletedTask);

        // Act
        var actionResult = await _handler.HandleAsync(beneficiaryDto);

        // Assert
        var result = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = result.Value.Should().BeOfType<Result<BeneficiaryDTO>>().Subject;
        returnValue.IsSuccess.Should().BeTrue();
        returnValue.Value.Should().Be(mappedBeneficiaryDto);
        
        _mockLogService.Verify(l => l.CreateLog(
            _mockHttpContext.Object,
            "Beneficiario agregado con éxito",
            ActionStatus.Success,
            userId,
            null,
            default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var beneficiaryDto = new BeneficiaryDTO { UserId = "invalid-guid" };

        // Act
        var actionResult = await _handler.HandleAsync(beneficiaryDto);

        // Assert
        var result = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnValue = result.Value.Should().BeOfType<Result<BeneficiaryResponseDTO>>().Subject;
        returnValue.IsSuccess.Should().BeFalse();
        returnValue.Errors.Should().Contain("El formato del UserId no es válido");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUser_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryDto = new BeneficiaryDTO { UserId = userId.ToString() };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync((User?)null);

        // Act
        var actionResult = await _handler.HandleAsync(beneficiaryDto);

        // Assert
        var result = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnValue = result.Value.Should().BeOfType<Result<BeneficiaryResponseDTO>>().Subject;
        returnValue.IsSuccess.Should().BeFalse();
        returnValue.Errors.Should().Contain("El usuario no fue encontrado");
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryDto = new BeneficiaryDTO { UserId = userId.ToString() };
        var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };
        var beneficiary = new Beneficiary();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<Beneficiary>(beneficiaryDto))
            .Returns(beneficiary);
        _mockUserRepository.Setup(r => r.UpdateAsync(user, default))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var actionResult = await _handler.HandleAsync(beneficiaryDto);

        // Assert
        var result = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnValue = result.Value.Should().BeOfType<Result<BeneficiaryResponseDTO>>().Subject;
        returnValue.IsSuccess.Should().BeFalse();
        returnValue.Errors.Should().Contain("Ha ocurrido un error al agregar el beneficiario");
        
        _mockLogService.Verify(l => l.CreateLog(
            _mockHttpContext.Object,
            "Ha ocurrido un error al agregar el beneficiario",
            ActionStatus.Error,
            null,
            It.IsAny<string>(),
            default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenMapperThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryDto = new BeneficiaryDTO { UserId = userId.ToString() };
        var user = new User { Id = userId, Beneficiaries = new List<Beneficiary>() };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<Beneficiary>(beneficiaryDto))
            .Throws(new AutoMapperMappingException("Mapping error"));

        // Act
        var actionResult = await _handler.HandleAsync(beneficiaryDto);

        // Assert
        var result = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnValue = result.Value.Should().BeOfType<Result<BeneficiaryResponseDTO>>().Subject;
        returnValue.IsSuccess.Should().BeFalse();
        returnValue.Errors.Should().Contain("Ha ocurrido un error al agregar el beneficiario");
        
        _mockLogService.Verify(l => l.CreateLog(
            _mockHttpContext.Object,
            "Ha ocurrido un error al agregar el beneficiario",
            ActionStatus.Error,
            null,
            It.IsAny<string>(),
            default), Times.Once);
    }
} 