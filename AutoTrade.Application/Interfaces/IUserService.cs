using AutoTrade.Application.DTOs;

namespace AutoTrade.Application.Interfaces;

public interface IUserService
{
    Task<CustomResponseDto<UserDto>> CreateUserAsync(UserCreateDto dto);
    Task<CustomResponseDto<UserDto>> GetByIdAsync(Guid id);
    Task<CustomResponseDto<List<UserDto>>> GetAllAsync();
}