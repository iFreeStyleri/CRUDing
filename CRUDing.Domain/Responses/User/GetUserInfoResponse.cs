using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities.Enums;

namespace CRUDing.Domain.Responses.User
{
    public record class GetUserInfoResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Role Role { get; set; }
        public int CountOrder { get; set; }
        public int CompletedCountOrder { get; set; }
        public DateTime Created { get; set; }
        public DateTime DateOfBirthday { get; set; }

        public GetUserInfoResponse(Entities.User user, int countOrder, int completedCountOrder)    
        {
            Id = user.Id;
            Email = user.Email;
            Name = user.Name;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            Role = user.Role;
            CountOrder = countOrder;
            CompletedCountOrder = completedCountOrder;
            Created = user.Created;
            DateOfBirthday = user.DateOfBirthday;
        }
    }
}
