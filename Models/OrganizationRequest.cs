using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Models
{
    public class OrganizationRequest
    {
        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserDescription { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserPassword { get; set; }
    }
}
