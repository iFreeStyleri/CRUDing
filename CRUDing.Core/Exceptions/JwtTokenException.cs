using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Core.Exceptions
{
    public class JwtTokenException : Exception
    {
        public JwtTokenException(string message) : base(message)
        {
            
        }
    }
}
