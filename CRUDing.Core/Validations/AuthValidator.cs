using CRUDing.Domain.DTOs.Users;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class AuthValidator : AbstractValidator<AuthorizeDTO>
    {
        public AuthValidator()
        {
            RuleFor(r => r.Email).EmailAddress().WithMessage("The email is in the wrong format");
            RuleFor(r => r.Password).NotNull().Length(6, 24).WithMessage("Password must been lenght between 6 and 24");
        }
    }
}
