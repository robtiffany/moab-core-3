using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class Telemetry
    {
        public Guid Id { get; set; }
        public Guid DigitalTwin { get; set; }
        public string DigitalTwinData { get; set; }
        public Guid DigitalTwinModel { get; set; }
        public DateTime Created { get; set; }
    }
}
