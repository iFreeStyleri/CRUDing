using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Users
{
    public record class RegisterDTO(
        string Email,
        string Password,
        string Name,
        string LastName,
        string? Patronymic,
        DateTime? DateOfBirthday);

}
