using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;

namespace AutoTrade.Core.Services;

public interface ITokenService
{
    TokenDto CreateToken(User user);
}