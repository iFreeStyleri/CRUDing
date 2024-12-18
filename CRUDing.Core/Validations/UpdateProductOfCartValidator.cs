using CRUDing.Domain.DTOs.Cart;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class UpdateProductOfCartValidator : AbstractValidator<UpdateProductOfCartDTO>
    {
        public UpdateProductOfCartValidator()
        {
            RuleFor(f => f.count).Must(s => s > 0);
        }
    }
}
