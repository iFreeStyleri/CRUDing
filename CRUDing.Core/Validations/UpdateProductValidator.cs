using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.DTOs.Products;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductValidator()
        {
            RuleFor(r => r.Id).Must(m => m > 0);
            RuleFor(r => r.Cost).NotNull();
            RuleFor(r => r.Cost.Currency).MaximumLength(3).NotNull().NotEmpty().Must(s => s == "$" || s == "₽");
            RuleFor(r => r.Cost.Value).NotNull().Must(m => m > 0);
            RuleFor(r => r.Description).MinimumLength(10).MaximumLength(1024);
            RuleFor(r => r.categoryId).NotNull();
        }
    }
}
