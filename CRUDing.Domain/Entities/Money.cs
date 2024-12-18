using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Entities
{
    public record class Money(string Currency,decimal Value);
}
