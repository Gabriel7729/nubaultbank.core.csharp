using NuBaultBank.Core.Enums;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.SharedKernel;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Core.Entities.ProductAggregate;
public class Account : EntityBase, IAggregateRoot
{
  public string AccountNumber { get; set; } = string.Empty;
  public decimal Balance { get; private set; }
  public AccountType AccountType { get; set; }
  public Guid UserId { get; set; }

  public static async Task<string> GenerateAccountNumberAsync(IProductService productService)
  {
    int lastSequence = 100;
    Random random = new Random();

    string accountNumber;
    do
    {
      string sequencePart = (lastSequence++ % 900 + 100).ToString();
      string randomPart = random.Next(0, 10000000).ToString("D7");

      accountNumber = sequencePart + randomPart;
    }
    while (!await productService.IsAnUniqueAccountAsync(accountNumber));

    return accountNumber;
  }
  public void Deposit(decimal amount)
  {
    if (amount <= 0)
      throw new ArgumentException("El monto a depositar debe ser mayor a 0");

    Balance += amount;
  }
  public void Withdraw(decimal amount)
  {
    if (amount <= 0)
      throw new ArgumentException("El monto debe ser mayor a 0");

    if (amount > Balance)
      throw new InvalidOperationException("Fondos insuficientes");

    Balance -= amount;
  }
  public Account Transfer(Account destinationAccount, decimal amount)
  {
    if (amount <= 0)
      throw new ArgumentException("El monto a transferir debe ser mayor a 0");

    if (amount > Balance)
      throw new InvalidOperationException("Fondos insuficientes");

    Balance -= amount;
    destinationAccount.Balance += amount;
    return destinationAccount;
  }
}
