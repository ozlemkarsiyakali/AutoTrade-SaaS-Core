using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTrade.Core.Entities
{
    public class VehicleBrand : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
    }
}
