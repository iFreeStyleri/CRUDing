using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Common.Jwt
{
    public class JwtTokens
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
