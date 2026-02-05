using FluentValidation;
using TicketHub.API.DTOs.Category;

namespace TicketHub.API.Validators.Category
{
    public class CategoryPostValidator : AbstractValidator<CategoryPostDto>
    {
        public CategoryPostValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The category name is required");
            RuleFor(x => x.Name).MinimumLength(3).
                WithMessage("The category name must be at least 3 characters long");
            RuleFor(x => x.Name).MaximumLength(25).
                WithMessage("The category name must have a maximum of 25 characters.");
        }
    }
}