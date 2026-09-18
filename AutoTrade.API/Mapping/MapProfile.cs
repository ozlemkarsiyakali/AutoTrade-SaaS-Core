using AutoMapper;
using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;

namespace AutoTrade.API.Mapping;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<VehicleBrand, VehicleBrandDto>().ReverseMap();
        CreateMap<CreateVehicleBrandDto, VehicleBrand>();
    }
}