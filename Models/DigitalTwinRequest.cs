using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class DigitalTwinRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid DigitalTwinModel { get; set; }
        public Guid Group { get; set; }
        public long Enabled { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
