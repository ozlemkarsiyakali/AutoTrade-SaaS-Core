using AutoMapper;
using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;

namespace AutoTrade.API.Mapping;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<VehicleBrand, VehicleBrandDto>().ReverseMap();
        CreateMap<CreateVehicleBrandDto, VehicleBrand>();
        CreateMap<UpdateVehicleBrandDto, VehicleBrand>();
    }
}