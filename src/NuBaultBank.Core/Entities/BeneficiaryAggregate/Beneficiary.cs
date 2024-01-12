using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBaultBank.SharedKernel.Interfaces;
using NuBaultBank.SharedKernel;

namespace NuBaultBank.Core.Entities.BeneficiaryAggregate;
public class Beneficiary : EntityBase, IAggregateRoot
{
  public string Name { get; set; } = string.Empty;
  public string PhoneNumber { get; set; } = string.Empty; 
  public string Email { get; set; } = string.Empty; 
  public string AccountNumber { get; set; } = string.Empty; 
  public string AccountType { get; set; } = string.Empty;
  public string BankName { get; set; } = string.Empty; 

}
