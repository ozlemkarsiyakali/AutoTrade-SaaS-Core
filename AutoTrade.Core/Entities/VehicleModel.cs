using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTrade.Core.Entities
{
    public class VehicleModel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;  

        public Guid VehicleBrandId { get; set; }
        public VehicleBrand VehicleBrand { get; set; } = null!;

        // Araç Tipi İlişkisi (Otomobil, SUV, Tır vb.)
        public Guid VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;

        public ICollection<VehicleModelNode> Nodes { get; set; } = new List<VehicleModelNode>();
    }
}
