namespace NuBaultBank.Core.Interfaces;
public interface IProductService
{
  Task<bool> IsAnUniqueAccountAsync(string accountNumber);
}
