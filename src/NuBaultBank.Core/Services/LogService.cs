using Microsoft.AspNetCore.Http;
using NuBaultBank.Core.Entities.LogAggregate;
using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Core.Models;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Services;
public class LogService : ILogService
{
  private readonly IRepository<Log> _logRepository;
  private readonly IAuthService _authService;
  public LogService(
    IRepository<Log> logRepository,
    IAuthService authService)
  {
    _logRepository = logRepository;
    _authService = authService;
  }
  public async Task CreateLog(HttpContext context, string message, ActionStatus actionStatus, Guid? userId = null, string? exceptionMessage = null, CancellationToken cancellationToken = default)
  {
    var userData = GetUserDataFromClaims(context);
    Log log = new()
    {
      Message = message,
      UserName = userData.UserName,
      ActionStatus = actionStatus,
      UserId = userId,
      ExceptionMessage = exceptionMessage
    };
    await _logRepository.AddAsync(log, cancellationToken);
  }

  public async Task CreateLog(string username, string message, ActionStatus actionStatus, Guid? userId = null, string? exceptionMessage = null, CancellationToken cancellationToken = default)
  {
    Log log = new()
    {
      Message = message,
      UserName = username,
      ActionStatus = actionStatus,
      UserId = userId,
      ExceptionMessage = exceptionMessage
    };
    await _logRepository.AddAsync(log, cancellationToken);
  }

  private TokenClaim GetUserDataFromClaims(HttpContext context)
  {
    string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    return _authService.DecryptToken(token);
  }
}

