using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;

namespace AutoTrade.Application.Services;

public interface ITokenService
{
    TokenDto CreateToken(User user);
}