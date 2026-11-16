using AutoTrade.Application.DTOs;

namespace AutoTrade.Application.Services;

public interface IAuthService
{
    Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto);
    Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto);
}