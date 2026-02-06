using FluentValidation;
using TicketHub.API.DTOs.Ticket;

namespace TicketHub.API.Validators.Ticket
{
    public class TicketPostValidator : AbstractValidator<TicketPostDto>
    {
        public TicketPostValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0).WithMessage("The event id must be greater than zero");
        }
    }
}