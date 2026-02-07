using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.API.DTOs.Event;
using TicketHub.API.Pagination;
using TicketHub.API.Services.Interfaces;
using TicketHub.API.Validators.Event;

namespace TicketHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IValidator<EventPostDto> _postValidator;
        private readonly IValidator<EventPutDto> _putValidator;

        public EventController(IEventService eventService,
            IValidator<EventPostDto> postValidator,
            IValidator<EventPutDto> putValidator)
        {
            _eventService = eventService;
            _postValidator = postValidator;
            _putValidator = putValidator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<EventDto>>> GetEvents([FromQuery] PaginationParams pParams)
        {
            var pagedEvents = await _eventService.GetEvents(pParams);
            return Ok(pagedEvents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var evnt = await _eventService.GetEvent(id);

            if (evnt == null)
                return NotFound();

            return Ok(evnt);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventPostDto eventPostDto)
        {
            var validation = await _postValidator.ValidateAsync(eventPostDto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var evnt = await _eventService.CreateEvent(eventPostDto);

            if (evnt == null)
                return Conflict();

            return CreatedAtAction(nameof(GetEvent), new { Id = evnt.Id }, evnt);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateEvent(EventPutDto eventPutDto)
        {
            var validation = await _putValidator.ValidateAsync(eventPutDto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _eventService.UpdateEvent(eventPutDto);

            if (!result)
                return Conflict();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var result = await _eventService.DeleteEvent(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}