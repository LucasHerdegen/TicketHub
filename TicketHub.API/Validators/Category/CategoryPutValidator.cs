using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketHub.API.DTOs.Category;

namespace TicketHub.API.Validators.Category
{
    public class CategoryPutValidator : AbstractValidator<CategoryPutDto>
    {
        public CategoryPutValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("The id must be greater than zero");
            RuleFor(x => x.Name).NotEmpty().WithMessage("The category name is required");
        }
    }
}