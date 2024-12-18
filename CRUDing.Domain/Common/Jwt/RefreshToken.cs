using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Common.Jwt
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
