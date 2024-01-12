using AutoMapper;
using NuBaultBank.Core.Entities.BeneficiaryAggregate;
using NuBaultBank.Core.Entities.LogAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Infrastructure.Dto.BeneficiaryDtos;
using NuBaultBank.Infrastructure.Dto.LogDtos;
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

    CreateMap<Log, LogResponseDto>().ReverseMap();
  }
}
