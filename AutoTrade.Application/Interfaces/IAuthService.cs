using AutoTrade.Application.DTOs;

namespace AutoTrade.Application.Interfaces;

public interface IAuthService
{
    Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto);
    Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto);
}