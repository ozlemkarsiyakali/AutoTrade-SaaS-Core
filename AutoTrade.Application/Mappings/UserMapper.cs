using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AutoTrade.Application.Mappings;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserMapper
{
    // Roller ayrı tabloda tutulduğu için servis katmanında doldurulur
    [MapperIgnoreTarget(nameof(UserDto.RoleId))]
    [MapperIgnoreTarget(nameof(UserDto.RoleName))]
    public static partial UserDto ToDto(this User source);
}