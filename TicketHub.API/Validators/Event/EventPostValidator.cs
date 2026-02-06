using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TicketHub.API.DTOs.Event;

namespace TicketHub.API.Validators.Event
{
    public class EventPostValidator : AbstractValidator<EventPostDto>
    {
        public EventPostValidator()
        {
            RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("The capacity cannot be zero");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("The category id must be greater than zero");
            RuleFor(x => x.Date).GreaterThan(DateTime.Now).WithMessage("The date has to be in the future");
            RuleFor(x => x.Name).NotEmpty().WithMessage("The name is required");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("The price must be greater or equal than zero");
        }
    }
}