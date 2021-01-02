using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class DigitalTwinModelStaticProperty
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid DigitalTwinModel { get; set; }
        public Guid Organization { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
