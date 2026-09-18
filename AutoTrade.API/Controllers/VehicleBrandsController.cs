using AutoMapper;
using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrade.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VehicleBrandsController : ControllerBase
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
        return Ok(CustomResponseDto<List<VehicleBrandDto>>.Success(200, brandDtos));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _service.GetByIdAsync(id);
        if (brand == null)
            return NotFound(CustomResponseDto<NoContentDto>.Fail(404, "Vehicle brand not found"));

        var brandDto = _mapper.Map<VehicleBrandDto>(brand);
        return Ok(CustomResponseDto<VehicleBrandDto>.Success(200, brandDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleBrandDto dto)
    {
        var brand = _mapper.Map<VehicleBrand>(dto);
        await _service.AddAsync(brand);
        var brandDto = _mapper.Map<VehicleBrandDto>(brand);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, CustomResponseDto<VehicleBrandDto>.Success(201, brandDto));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateVehicleBrandDto dto)
    {
        var brand = await _service.GetByIdAsync(dto.Id);
        if (brand == null)
            return NotFound(CustomResponseDto<NoContentDto>.Fail(404, "Vehicle brand not found"));

        _mapper.Map(dto, brand);
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