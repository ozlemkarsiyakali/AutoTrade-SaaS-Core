using AutoTrade.Core.DTOs;
using AutoTrade.Domain.Entities;

namespace AutoTrade.Core.Services;

public interface ITokenService
{
    TokenDto CreateToken(User user);
}