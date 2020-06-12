using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreBeginners.Models
{
    public static class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role","CreateRole"),
            new Claim("Edit Role","EditRole"),
            new Claim("Delete Role","DeleteRole"),
        };
    }
}
