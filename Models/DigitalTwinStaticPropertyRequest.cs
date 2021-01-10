using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class DigitalTwinStaticPropertyRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid DigitalTwin { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
