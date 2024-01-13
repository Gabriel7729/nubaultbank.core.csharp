using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using NuBaultBank.Core.Abstracts;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Core.Entities.UserAggregate.Specs;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NuBaultBank.Web.Endpoints.UserEndpoints;

public class PayLoan : EndpointBaseAsync.WithRequest<PayLoanRequest>.WithActionResult
{
  private readonly ILoanService _loanService;
  private readonly IRepository<User> _repostiroy;
  public PayLoan(ILoanService loanService)
  {
    _loanService = loanService;
  }
  [HttpGet("/api/User/Paginated")]
  [SwaggerOperation(
          Summary = "Make payment for a Loan of a user",
          Description = "Make payment for a Loan of a user",
          OperationId = "UserEndpoints.PayLoan",
          Tags = new[] { "UserEndpoints" })
  ]
  public override async Task<ActionResult> HandleAsync(PayLoanRequest request, CancellationToken cancellationToken = default)
  {
    // Make sure user exists
    GetUserWithSpec spec = new(request.UserId);
    User? user = await _repostiroy.GetBySpecAsync(spec, cancellationToken);
    if (user is null)
      return BadRequest(Result<GeneralResponse>.Error($"El usuario con el id {request.UserId} no existe."));

    // Make sure the Loan and the account is from the user
    Account? account = user.Accounts.FirstOrDefault(account => account.Id == request.AccountId);
    if(account is null)
      return BadRequest(Result<GeneralResponse>.Error($"La cuenta con el id: {request.AccountId} no existe o no pertenece al usuario."));

    Loan? loan = user.Loans.FirstOrDefault(loan => loan.Id == request.LoanId);
    if(loan is null)
      return BadRequest(Result<GeneralResponse>.Error($"La cuenta con el id: {request.LoanId} no existe o no pertenece al usuario."));

    // Call method to Pay Loan from specificed account from user (which also needs to be validated that is from the user the specified account)
    await _loanService.PayLoan(user, account, loan);
    throw new NotImplementedException();
  }
}
