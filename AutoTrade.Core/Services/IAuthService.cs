using AutoTrade.Core.DTOs;

namespace AutoTrade.Core.Services;

public interface IAuthService
{
    Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto);
    Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto);
}