using AutoMapper;
using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrade.API.Controllers;

public class VehicleBrandsController : CustomBaseController
{
    private readonly IService<VehicleBrand> _service;
    private readonly IMapper _mapper;

    public VehicleBrandsController(IService<VehicleBrand> service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _service.GetAllAsync();
        var brandDtos = _mapper.Map<List<VehicleBrandDto>>(brands);
        return CreateActionResult(brandDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null)
        {
            return CreateActionResult($"Brand with ID '{id}' was not found.", 404);
        }

        var brandDto = _mapper.Map<VehicleBrandDto>(brand);
        return CreateActionResult(brandDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleBrandDto createVehicleBrandDto)
    {
        var brand = _mapper.Map<VehicleBrand>(createVehicleBrandDto);
        var createdBrand = await _service.AddAsync(brand);
        var brandDto = _mapper.Map<VehicleBrandDto>(createdBrand);

        return CreateActionResult(brandDto, 201);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, VehicleBrandDto vehicleBrandDto)
    {
        if (id != vehicleBrandDto.Id)
        {
            return CreateActionResult("ID mismatch.", 400);
        }

        var existingBrand = await _service.GetByIdAsync(id);
        if (existingBrand == null)
        {
            return CreateActionResult($"Brand with ID '{id}' was not found.", 404);
        }

        _mapper.Map(vehicleBrandDto, existingBrand);
        await _service.UpdateAsync(existingBrand);

        return CreateActionResult(204);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null)
        {
            return CreateActionResult($"Brand with ID '{id}' was not found.", 404);
        }

        await _service.RemoveAsync(brand);
        return CreateActionResult(204);
    }
}