using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.DTOs.Category;
using FluentValidation;

namespace CRUDing.Core.Validations
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDTO>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(r => r.Name).MaximumLength(20).NotEmpty().NotNull();
        }
    }
}
