using TicketHub.API.DTOs;
using FluentValidation;

namespace TicketHub.API.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("The email is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("The password is required");
        }
    }
}