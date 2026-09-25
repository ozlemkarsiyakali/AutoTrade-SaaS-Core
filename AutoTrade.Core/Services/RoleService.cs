using System.Text.Json;
using AutoMapper;
using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Interfaces; // IGenericRepository burada yer alıyor
using AutoTrade.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace AutoTrade.Service.Services;

public class RoleService : IRoleService
{
    private readonly IGenericRepository<AppRole> _roleRepository;
    private readonly IGenericRepository<AppRoleClaim> _roleClaimRepository;
    private readonly IGenericRepository<AppUserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public RoleService(
        IGenericRepository<AppRole> roleRepository,
        IGenericRepository<AppRoleClaim> roleClaimRepository,
        IGenericRepository<AppUserRole> userRoleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDistributedCache cache)
    {
        _roleRepository = roleRepository;
        _roleClaimRepository = roleClaimRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CustomResponseDto<List<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.FindAsync(x => !x.IsDeleted);
        var allClaims = await _roleClaimRepository.FindAsync(x => !x.IsDeleted);

        var roleDtos = roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = allClaims.Where(c => c.RoleId == role.Id).Select(c => c.ClaimValue).ToList()
        }).ToList();

        return CustomResponseDto<List<RoleDto>>.Success(200, roleDtos);
    }

    public async Task<CustomResponseDto<RoleDto>> GetRoleByIdAsync(Guid id)
    {
        var roles = await _roleRepository.FindAsync(x => x.Id == id && !x.IsDeleted);
        var role = roles.FirstOrDefault();

        if (role == null)
            return CustomResponseDto<RoleDto>.Fail(404, "Role not found.");

        var claims = await _roleClaimRepository.FindAsync(x => x.RoleId == id && !x.IsDeleted);

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = claims.Select(c => c.ClaimValue).ToList()
        };

        return CustomResponseDto<RoleDto>.Success(200, roleDto);
    }

    public async Task<CustomResponseDto<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        var role = new AppRole
        {
            Name = createRoleDto.Name,
            Description = createRoleDto.Description
        };

        await _roleRepository.AddAsync(role);

        foreach (var permission in createRoleDto.Permissions.Distinct())
        {
            await _roleClaimRepository.AddAsync(new AppRoleClaim
            {
                RoleId = role.Id,
                ClaimType = "Permission",
                ClaimValue = permission
            });
        }

        await _unitOfWork.CommitAsync();

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = createRoleDto.Permissions
        };

        return CustomResponseDto<RoleDto>.Success(201, roleDto);
    }

    public async Task<CustomResponseDto<NoContentDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
    {
        var roles = await _roleRepository.FindAsync(x => x.Id == updateRoleDto.Id && !x.IsDeleted);
        var role = roles.FirstOrDefault();

        if (role == null)
            return CustomResponseDto<NoContentDto>.Fail(404, "Role not found.");

        role.Name = updateRoleDto.Name;
        role.Description = updateRoleDto.Description;
        role.UpdatedAt = DateTime.UtcNow;

        // Mevcut claim'leri pasife al (Soft Delete)
        var existingClaims = await _roleClaimRepository.FindAsync(x => x.RoleId == role.Id && !x.IsDeleted);
        foreach (var claim in existingClaims)
        {
            claim.IsDeleted = true;
            claim.UpdatedAt = DateTime.UtcNow;
            _roleClaimRepository.Update(claim);
        }

        // Yeni izinleri ekle
        foreach (var permission in updateRoleDto.Permissions.Distinct())
        {
            await _roleClaimRepository.AddAsync(new AppRoleClaim
            {
                RoleId = role.Id,
                ClaimType = "Permission",
                ClaimValue = permission
            });
        }

        _roleRepository.Update(role);
        await _unitOfWork.CommitAsync();

        await InvalidateUsersCacheByRoleIdAsync(role.Id);

        return CustomResponseDto<NoContentDto>.Success(204);
    }

    public async Task<CustomResponseDto<NoContentDto>> DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null || role.IsDeleted)
            return CustomResponseDto<NoContentDto>.Fail(404, "Role not found.");

        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;

        _roleRepository.Update(role);
        await _unitOfWork.CommitAsync();

        await InvalidateUsersCacheByRoleIdAsync(id);

        return CustomResponseDto<NoContentDto>.Success(204);
    }

    public async Task<CustomResponseDto<NoContentDto>> AssignRoleToUserAsync(AssignRoleToUserDto assignRoleToUserDto)
    {
        var existingUserRoles = await _userRoleRepository.FindAsync(x => x.UserId == assignRoleToUserDto.UserId && !x.IsDeleted);

        foreach (var userRole in existingUserRoles)
        {
            userRole.IsDeleted = true;
            userRole.UpdatedAt = DateTime.UtcNow;
            _userRoleRepository.Update(userRole);
        }

        foreach (var roleId in assignRoleToUserDto.RoleIds.Distinct())
        {
            await _userRoleRepository.AddAsync(new AppUserRole
            {
                UserId = assignRoleToUserDto.UserId,
                RoleId = roleId
            });
        }

        await _unitOfWork.CommitAsync();

        await _cache.RemoveAsync($"user_permissions:{assignRoleToUserDto.UserId}");

        return CustomResponseDto<NoContentDto>.Success(204);
    }

    public async Task<CustomResponseDto<List<string>>> GetUserPermissionsAsync(Guid userId)
    {
        string cacheKey = $"user_permissions:{userId}";

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var permissionsFromCache = JsonSerializer.Deserialize<List<string>>(cachedData);
            return CustomResponseDto<List<string>>.Success(200, permissionsFromCache ?? new());
        }

        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId && !ur.IsDeleted);
        var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var allClaims = await _roleClaimRepository.FindAsync(rc => userRoleIds.Contains(rc.RoleId) && !rc.IsDeleted);
        var permissions = allClaims.Select(rc => rc.ClaimValue).Distinct().ToList();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(permissions), cacheOptions);

        return CustomResponseDto<List<string>>.Success(200, permissions);
    }

    private async Task InvalidateUsersCacheByRoleIdAsync(Guid roleId)
    {
        var affectedUserRoles = await _userRoleRepository.FindAsync(ur => ur.RoleId == roleId && !ur.IsDeleted);
        var affectedUserIds = affectedUserRoles.Select(ur => ur.UserId).Distinct().ToList();

        foreach (var userId in affectedUserIds)
        {
            await _cache.RemoveAsync($"user_permissions:{userId}");
        }
    }
}