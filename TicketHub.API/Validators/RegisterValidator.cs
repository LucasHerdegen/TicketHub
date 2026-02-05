using TicketHub.API.DTOs;
using FluentValidation;

namespace TicketHub.API.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("The first name must not be empty");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("The last name must not be empty");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Incorrect Email format");
            RuleFor(x => x.Password).MinimumLength(4).WithMessage("The password must be at least 4 characters longs");
        }
    }
}