using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Users
{
    public record class AuthorizeDTO(string Email, string Password);
}
