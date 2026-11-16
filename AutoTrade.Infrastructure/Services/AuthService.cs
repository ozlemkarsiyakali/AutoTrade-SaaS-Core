using AutoMapper;
using AutoTrade.Core;
using AutoTrade.Core.DTOs;
using AutoTrade.Domain.Entities;
using AutoTrade.Core.Interfaces;
using AutoTrade.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AutoTrade.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<AppRole> _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<AppRole> roleRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<ResponseDto<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null)
        {
            return ResponseDto<TokenDto>.Fail("Giriş bilgileri boş olamaz.", StatusCodes.Status400BadRequest);
        }

        var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email && !u.IsDeleted);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
        {
            return ResponseDto<TokenDto>.Fail("E-posta veya şifre hatalı.", StatusCodes.Status404NotFound);
        }

        // Şifre doğrulama (PasswordHasher ile)
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return ResponseDto<TokenDto>.Fail("E-posta veya şifre hatalı.", StatusCodes.Status400BadRequest);
        }

        var token = _tokenService.CreateToken(user);
        return ResponseDto<TokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<ResponseDto<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        var existingUsers = await _userRepository.FindAsync(u => u.Email == registerDto.Email && !u.IsDeleted);
        if (existingUsers.Any())
        {
            return ResponseDto<UserDto>.Fail("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.", StatusCodes.Status400BadRequest);
        }

        // Varsayılan rol "Viewer" olarak aranıyor
        var roles = await _roleRepository.FindAsync(r => r.Name == "Viewer" && !r.IsDeleted);
        var defaultRole = roles.FirstOrDefault();

        if (defaultRole == null)
        {
            return ResponseDto<UserDto>.Fail("Sistemde varsayılan 'Viewer' rolü bulunamadı.", StatusCodes.Status500InternalServerError);
        }

        var newUser = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            IsActive = true,
            RoleId = defaultRole.Id
        };

        // Şifre hash'leme
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);

        await _userRepository.AddAsync(newUser);
        await _unitOfWork.CommitAsync();

        var userDto = _mapper.Map<UserDto>(newUser);

        return ResponseDto<UserDto>.Success(userDto, StatusCodes.Status201Created);
    }
}