using AutoTrade.Core.DTOs;
using AutoTrade.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrade.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : CustomBaseController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _roleService.GetAllRolesAsync();
        return CreateActionResult(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _roleService.GetRoleByIdAsync(id);
        return CreateActionResult(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDto createRoleDto)
    {
        var response = await _roleService.CreateRoleAsync(createRoleDto);
        return CreateActionResult(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateRoleDto updateRoleDto)
    {
        var response = await _roleService.UpdateRoleAsync(updateRoleDto);
        return CreateActionResult(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        return CreateActionResult(response);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(AssignRoleToUserDto assignRoleToUserDto)
    {
        var response = await _roleService.AssignRoleToUserAsync(assignRoleToUserDto);
        return CreateActionResult(response);
    }

    [HttpGet("user-permissions/{userId:guid}")]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        var response = await _roleService.GetUserPermissionsAsync(userId);
        return CreateActionResult(response);
    }
}