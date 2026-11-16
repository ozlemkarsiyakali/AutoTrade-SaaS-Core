using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AutoTrade.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<AppRole> _roleRepository;
    private readonly IGenericRepository<AppUserRole> _userRoleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IGenericRepository<User> userRepository,
        IGenericRepository<AppRole> roleRepository,
        IGenericRepository<AppUserRole> userRoleRepository,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomResponseDto<UserDto>> CreateUserAsync(UserCreateDto dto)
    {
        // 1. Email kontrolü (Silinmişler dahil veya hariç)
        var existingUser = await _userRepository.FindAsync(x => x.Email.ToLower() == dto.Email.ToLower() && !x.IsDeleted);
        if (existingUser.Any())
            return CustomResponseDto<UserDto>.Fail(400, "Bu e-posta adresi zaten kullanılmaktadır.");

        // 2. Rol var mı kontrolü
        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role == null || role.IsDeleted)
            return CustomResponseDto<UserDto>.Fail(404, "Belirtilen rol bulunamadı.");

        // 3. Kullanıcı oluşturma
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            RoleId = dto.RoleId
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);

        await _userRepository.AddAsync(newUser);

        // 4. Rol ataması
        await _userRoleRepository.AddAsync(new AppUserRole
        {
            Id = Guid.NewGuid(), // AppUserRole BaseEntity'den türuyorsa Id atanmalı
            UserId = newUser.Id,
            RoleId = role.Id
        });

        await _unitOfWork.CommitAsync();

        var userDto = new UserDto
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            RoleId = role.Id,
            RoleName = role.Name
        };

        return CustomResponseDto<UserDto>.Success(201, userDto);
    }
    public async Task<CustomResponseDto<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted)
            return CustomResponseDto<UserDto>.Fail(404, "Kullanıcı bulunamadı.");

        var userRoles = await _userRoleRepository.FindAsync(x => x.UserId == id && !x.IsDeleted);
        var roleId = userRoles.Select(x => x.RoleId).FirstOrDefault();
        var role = await _roleRepository.GetByIdAsync(roleId);

        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            RoleId = roleId,
            RoleName = role?.Name ?? string.Empty
        };

        return CustomResponseDto<UserDto>.Success(200, userDto);
    }

    public async Task<CustomResponseDto<List<UserDto>>> GetAllAsync()
    {
        var users = await _userRepository.FindAsync(x => !x.IsDeleted);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var userRoles = await _userRoleRepository.FindAsync(x => x.UserId == user.Id && !x.IsDeleted);
            var roleId = userRoles.Select(x => x.RoleId).FirstOrDefault();
            var role = await _roleRepository.GetByIdAsync(roleId);

            userDtos.Add(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = roleId,
                RoleName = role?.Name ?? string.Empty
            });
        }

        return CustomResponseDto<List<UserDto>>.Success(200, userDtos);
    }
}