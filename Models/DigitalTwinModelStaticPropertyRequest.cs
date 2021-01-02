using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class DigitalTwinModelStaticPropertyRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid DigitalTwinModel { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
