using AutoMapper;
using NuBaultBank.Core.Entities.LogAggregate;
using NuBaultBank.Core.Entities.UserAggregate;
using NuBaultBank.Infrastructure.Dto.LogDtos;
using NuBaultBank.Infrastructure.Dto.UserDtos;

namespace NuBaultBank.Infrastructure.Config;
public class MainMapperProfile : Profile
{
  public MainMapperProfile()
  {
    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<User, UserResponseDto>().ReverseMap();

    CreateMap<Log, LogResponseDto>().ReverseMap();
  }
}
