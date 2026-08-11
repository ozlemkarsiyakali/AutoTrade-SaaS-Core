using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTrade.Core.Entities
{
    public class VehicleType : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // Örn: "Otomobil", "SUV", "Tır", "Motosiklet", "Hafif Ticari"

        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
    }
}
