using AutoTrade.Core.DTOs;

namespace AutoTrade.Core.Services;

public interface IRoleService
{
    Task<CustomResponseDto<List<RoleDto>>> GetAllRolesAsync();
    Task<CustomResponseDto<RoleDto>> GetRoleByIdAsync(Guid id);
    Task<CustomResponseDto<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<CustomResponseDto<NoContentDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
    Task<CustomResponseDto<NoContentDto>> DeleteRoleAsync(Guid id);
    Task<CustomResponseDto<NoContentDto>> AssignRoleToUserAsync(AssignRoleToUserDto assignRoleToUserDto);
    Task<CustomResponseDto<List<string>>> GetUserPermissionsAsync(Guid userId);
}