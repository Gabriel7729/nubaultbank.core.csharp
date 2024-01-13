using AutoMapper;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.LogAggregate;
using NuBaultBank.Core.Entities.ProductAggregate;
using NuBaultBank.Core.Entities.TransferAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.Infrastructure.Dto.LogDtos;
using NuBaultBank.Infrastructure.Dto.ProductDtos;
using NuBaultBank.Infrastructure.Dto.TransferDtos;
using NuBaultBank.Infrastructure.Dto.UserDtos;

namespace NuBaultBank.Infrastructure.Config;
public class MainMapperProfile : Profile
{
  public MainMapperProfile()
  {
    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<User, UserResponseDto>().ReverseMap();

    CreateMap<Beneficiary, BeneficiaryDTO>().ReverseMap();
    CreateMap<Beneficiary, BeneficiaryResponseDTO>().ReverseMap();

    CreateMap<Account, AccountDto>().ReverseMap();
    CreateMap<Account, AccountResponseDto>().ReverseMap();

    CreateMap<Loan, LoanDto>().ReverseMap();
    CreateMap<Loan, LoanResponseDto>().ReverseMap();

    CreateMap<Transfer, TransferDto>().ReverseMap();
    CreateMap<Transfer, TransferResponseDto>().ReverseMap();

    CreateMap<Log, LogResponseDto>().ReverseMap();
  }
}
