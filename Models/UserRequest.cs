using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class UserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public long Role { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
