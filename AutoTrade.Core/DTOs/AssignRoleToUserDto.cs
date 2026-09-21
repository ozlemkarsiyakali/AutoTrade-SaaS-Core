namespace AutoTrade.Core.DTOs;

public class AssignRoleToUserDto
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}