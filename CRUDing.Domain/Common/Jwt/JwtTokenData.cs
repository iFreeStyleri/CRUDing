using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities.Enums;

namespace CRUDing.Core.Implementations.Services
{
    public class JwtTokenData
    {
        public string Email { get; set; }
        public string Ip { get; set; }
        public Role Role { get; set; }
    }
}
