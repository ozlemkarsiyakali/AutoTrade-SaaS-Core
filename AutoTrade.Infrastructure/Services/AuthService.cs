using AutoTrade.Application.DTOs;
using AutoTrade.Application.Interfaces;
using AutoTrade.Application.Mappings;
using AutoTrade.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AutoTrade.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const string DefaultRoleName = "User";

    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<AppRole> _roleRepository;
    private readonly IGenericRepository<AppUserRole> _userRoleRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<AppRole> roleRepository,
        IGenericRepository<AppUserRole> userRoleRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        {
            return ResponseDto<TokenDto>.Fail("Giriş bilgileri boş olamaz.", StatusCodes.Status400BadRequest);
        }

        var email = loginDto.Email.Trim().ToLowerInvariant();
        var users = await _userRepository.FindAsync(u => u.Email == email);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
        {
            return InvalidCredentials();
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return InvalidCredentials();
        }

        var token = _tokenService.CreateToken(user);
        return ResponseDto<TokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto == null
            || string.IsNullOrWhiteSpace(registerDto.Email)
            || string.IsNullOrEmpty(registerDto.Password))
        {
            return ResponseDto<UserDto>.Fail("E-posta ve şifre zorunludur.", StatusCodes.Status400BadRequest);
        }

        var email = registerDto.Email.Trim().ToLowerInvariant();

        var existingUsers = await _userRepository.FindAsync(u => u.Email == email);
        if (existingUsers.Any())
        {
            return ResponseDto<UserDto>.Fail("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.", StatusCodes.Status400BadRequest);
        }

        var roles = await _roleRepository.FindAsync(r => r.Name == DefaultRoleName);
        var defaultRole = roles.FirstOrDefault();
        if (defaultRole == null)
        {
            return ResponseDto<UserDto>.Fail($"Sistemde varsayılan '{DefaultRoleName}' rolü bulunamadı.", StatusCodes.Status500InternalServerError);
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = (registerDto.FirstName ?? string.Empty).Trim(),
            LastName = (registerDto.LastName ?? string.Empty).Trim(),
            Email = email,
            IsActive = true
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);

        await _userRepository.AddAsync(newUser);
        await _userRoleRepository.AddAsync(new AppUserRole
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            RoleId = defaultRole.Id
        });

        // Kullanıcı ve rol ataması tek SaveChanges (tek transaction) ile kaydedilir
        await _unitOfWork.CommitAsync();

        var userDto = newUser.ToDto();
        userDto.RoleId = defaultRole.Id;
        userDto.RoleName = defaultRole.Name;

        return ResponseDto<UserDto>.Success(userDto, StatusCodes.Status201Created);
    }

    private static ResponseDto<TokenDto> InvalidCredentials()
    {
        return ResponseDto<TokenDto>.Fail("E-posta veya şifre hatalı.", StatusCodes.Status401Unauthorized);
    }
}