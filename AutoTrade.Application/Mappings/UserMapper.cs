using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AutoTrade.Application.Mappings;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreTarget(nameof(UserDto.RoleName))]
    public static partial UserDto ToDto(this User source);
}