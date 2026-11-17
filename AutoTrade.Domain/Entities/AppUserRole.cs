namespace AutoTrade.Domain.Entities;

public class AppUserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    // Navigation Property
    public AppRole Role { get; set; } = null!;
}