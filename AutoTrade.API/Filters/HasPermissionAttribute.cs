using Microsoft.AspNetCore.Authorization;

namespace AutoTrade.API.Filters;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
    }
}