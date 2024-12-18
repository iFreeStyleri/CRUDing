using System.Data;
using CRUDing.Domain.DTOs.Users;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.Email).NotNull().EmailAddress();
            RuleFor(r => r.Name).NotNull().Length(4,24);
            RuleFor(r => r.LastName).NotNull().Length(4, 30);
            RuleFor(r => r.Patronymic).MaximumLength(30);
            RuleFor(r => r.Password).NotNull().Length(6, 24);
            RuleFor(r => r.DateOfBirthday).NotNull();
        }
    }
}
