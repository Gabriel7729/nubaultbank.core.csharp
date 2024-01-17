using Ardalis.Specification;

namespace NuBaultBank.Core.Entities.TransferAggregate.Specs;
public class GetTransferFromUserSpec : Specification<Transfer>
{
  public GetTransferFromUserSpec(Guid userId)
  {
    Query
      .Where(transfer => transfer.UserId == userId);
  }
}
