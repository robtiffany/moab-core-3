using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public Guid SecurityToken { get; set; }
    }
}
