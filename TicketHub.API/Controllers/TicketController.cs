using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Pagination;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;
        private readonly IValidator<TicketPostDto> _postValidator;

        public TicketController(ITicketService service, IValidator<TicketPostDto> postValidator)
        {
            _service = service;
            _postValidator = postValidator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<TicketDto>>> GetTickets([FromQuery] PaginationParams pParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("The user could not be identified");

            var pTickets = await _service.GetTickets(pParams, userId);

            return Ok(pTickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("The user could not be identified");

            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var ticket = await _service.GetTicket(id, userId);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedList<TicketDto>>> GetAllTickets([FromQuery] PaginationParams pParams)
        {
            var pTickets = await _service.GetTickets(pParams);
            return Ok(pTickets);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketPostDto ticketPostDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("The user could not be identified");

            var validation = await _postValidator.ValidateAsync(ticketPostDto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var ticket = await _service.CreateTicket(ticketPostDto, userId);

            if (ticket == null)
                return Conflict();

            return CreatedAtAction(nameof(GetTicket), new { Id = ticket.Id }, ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("The user could not be identified");

            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var result = await _service.DeleteTicket(id, userId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}