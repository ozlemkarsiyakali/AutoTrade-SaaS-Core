using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;
using AutoTrade.Core;
using AutoTrade.Core.Services;
using Microsoft.AspNetCore.Http;

namespace AutoTrade.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null)
        {
            return ResponseDto<TokenDto>.Fail("Giriş bilgileri boş olamaz.", StatusCodes.Status400BadRequest);
        }

        // FindAsync kullanımı
        var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email && !u.IsDeleted);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
        {
            return ResponseDto<TokenDto>.Fail("E-posta veya şifre hatalı.", StatusCodes.Status404NotFound);
        }

        if (user.PasswordHash != loginDto.Password)
        {
            return ResponseDto<TokenDto>.Fail("E-posta veya şifre hatalı.", StatusCodes.Status400BadRequest);
        }

        var token = _tokenService.CreateToken(user);
        return ResponseDto<TokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        // FindAsync kullanımı
        var existingUsers = await _userRepository.FindAsync(u => u.Email == registerDto.Email && !u.IsDeleted);
        if (existingUsers.Any())
        {
            return ResponseDto<UserDto>.Fail("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.", StatusCodes.Status400BadRequest);
        }

        var roles = await _roleRepository.FindAsync(r => r.Name == "User" && !r.IsDeleted);
        var defaultRole = roles.FirstOrDefault();

        var newUser = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PasswordHash = registerDto.Password,
            IsActive = true,
            RoleId = defaultRole != null ? defaultRole.Id : Guid.Empty
        };

        await _userRepository.AddAsync(newUser);
        await _unitOfWork.CommitAsync();

        var userDto = new UserDto
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            IsActive = newUser.IsActive,
            RoleId = newUser.RoleId
        };

        return ResponseDto<UserDto>.Success(userDto, StatusCodes.Status201Created);
    }
}