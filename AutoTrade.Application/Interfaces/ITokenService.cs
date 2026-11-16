using AutoTrade.Application.DTOs;
using AutoTrade.Domain.Entities;

namespace AutoTrade.Application.Interfaces;

public interface ITokenService
{
    TokenDto CreateToken(User user);
}