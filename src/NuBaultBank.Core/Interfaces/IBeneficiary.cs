using Ardalis.Result;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Models;

namespace NuBaultBank.Core.Interfaces;

public interface IBeneficiary
{
  Task<Result<Beneficiary>> EditBeneficiaryAsync(Beneficiary requestEntity, CancellationToken cancellationToken = default);
}
