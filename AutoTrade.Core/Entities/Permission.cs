using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTrade.Core.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty;  
        public string Group { get; set; } = string.Empty;  
        public string Description { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
