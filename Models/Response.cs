using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class Response
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
