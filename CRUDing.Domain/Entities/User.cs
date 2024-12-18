using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;
using CRUDing.Domain.Entities.Enums;

namespace CRUDing.Domain.Entities
{
    public class User : Entity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Role Role { get; set; }
        public List<Order> Orders { get; set; }
        public Cart Cart { get; set; }
        public DateTime Created { get; set; }
        public DateTime DateOfBirthday { get; set; }
    }
}
