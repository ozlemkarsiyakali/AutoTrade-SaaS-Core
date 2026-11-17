using AutoTrade.Application.DTOs;
using AutoTrade.Application.Interfaces;
using AutoTrade.Application.Mappings;
using AutoTrade.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrade.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehicleBrandsController : ControllerBase
{
    private readonly IService<VehicleBrand> _service;

    public VehicleBrandsController(IService<VehicleBrand> service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _service.GetAllAsync();
        return Ok(CustomResponseDto<List<VehicleBrandDto>>.Success(200, brands.ToDtoList()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null)
            return NotFound(CustomResponseDto<NoContentDto>.Fail(404, "Vehicle brand not found"));

        return Ok(CustomResponseDto<VehicleBrandDto>.Success(200, brand.ToDto()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleBrandDto dto)
    {
        var brand = dto.ToEntity();
        await _service.AddAsync(brand);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, CustomResponseDto<VehicleBrandDto>.Success(201, brand.ToDto()));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateVehicleBrandDto dto)
    {
        var brand = await _service.GetByIdAsync(dto.Id);
        if (brand == null)
            return NotFound(CustomResponseDto<NoContentDto>.Fail(404, "Vehicle brand not found"));

        dto.ApplyTo(brand);
        await _service.UpdateAsync(brand);
        return Ok(CustomResponseDto<NoContentDto>.Success(204));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null || brand.IsDeleted)
            return NotFound(CustomResponseDto<NoContentDto>.Fail(404, "Vehicle brand not found"));

        brand.IsDeleted = true;
        brand.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateAsync(brand);

        return Ok(CustomResponseDto<NoContentDto>.Success(204));
    }
}