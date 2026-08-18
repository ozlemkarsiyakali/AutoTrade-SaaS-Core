using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTrade.Core.Entities
{
    public class VehicleModelNode : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Örn: "Sportback", "1.5 TFSI", "S Line"

        // Ana Modele olan bağlantı
        public Guid VehicleModelId { get; set; }
        public VehicleModel VehicleModel { get; set; } = null!;

        // Ağaç Hiyerarşisi
        public Guid? ParentId { get; set; }
        public VehicleModelNode? Parent { get; set; }
        public ICollection<VehicleModelNode> Children { get; set; } = new List<VehicleModelNode>();
    }
}
