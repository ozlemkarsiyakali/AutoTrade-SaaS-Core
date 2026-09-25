using System.Security.Claims;
using AutoTrade.Core.Services;
using AutoTrade.API.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace AutoTrade.API.Handlers;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        var permissionsResponse = await roleService.GetUserPermissionsAsync(userId);

        if (permissionsResponse.Data != null && permissionsResponse.Data.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}