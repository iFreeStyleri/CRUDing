using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.DTOs.Category;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class AddCategoryValidator : AbstractValidator<AddCategoryDTO>
    {
        public AddCategoryValidator()
        {
            RuleFor(r => r.Name).NotEmpty().NotNull().MaximumLength(20);
        }
    }
}
