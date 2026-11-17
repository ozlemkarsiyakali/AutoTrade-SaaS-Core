using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace AutoTrade.Application.Mappings;

[Mapper]
public static partial class VehicleBrandMapper
{
    public static partial VehicleBrandDto ToDto(this VehicleBrand source);

    public static partial List<VehicleBrandDto> ToDtoList(this IEnumerable<VehicleBrand> source);

    [MapperIgnoreTarget(nameof(VehicleBrand.Id))]
    [MapperIgnoreTarget(nameof(VehicleBrand.CreatedAt))]
    [MapperIgnoreTarget(nameof(VehicleBrand.UpdatedAt))]
    [MapperIgnoreTarget(nameof(VehicleBrand.IsDeleted))]
    [MapperIgnoreTarget(nameof(VehicleBrand.Models))]
    public static partial VehicleBrand ToEntity(this CreateVehicleBrandDto source);

    [MapperIgnoreTarget(nameof(VehicleBrand.CreatedAt))]
    [MapperIgnoreTarget(nameof(VehicleBrand.UpdatedAt))]
    [MapperIgnoreTarget(nameof(VehicleBrand.IsDeleted))]
    [MapperIgnoreTarget(nameof(VehicleBrand.Models))]
    public static partial void ApplyTo(this UpdateVehicleBrandDto source, VehicleBrand target);
}