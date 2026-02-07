using AutoMapper;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Models;
using TicketHub.API.Pagination;
using TicketHub.API.Repository;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.API.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository repository, IMapper mapper, IRepository<Event> eventRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        public async Task<PagedList<TicketDto>> GetTickets(PaginationParams pParams)
        {
            var pTickets = await _repository.GetTicketsWithEvent(pParams);
            var ticketsDto = _mapper.Map<IEnumerable<TicketDto>>(pTickets.Items);

            return new PagedList<TicketDto>(ticketsDto, pTickets.TotalCount, pTickets.CurrentPage, pTickets.PageSize);
        }

        public async Task<PagedList<TicketDto>> GetTickets(PaginationParams pParams, string userId)
        {
            var pTickets = await _repository.GetTicketsWithEvent(pParams, userId);
            var ticketsDto = _mapper.Map<IEnumerable<TicketDto>>(pTickets.Items);

            return new PagedList<TicketDto>(ticketsDto, pTickets.TotalCount, pTickets.CurrentPage, pTickets.PageSize);
        }

        public async Task<TicketDto?> GetTicket(int id, string userId)
        {
            var ticket = await _repository.GetTicketWithEvent(id, userId);

            if (ticket == null)
                return null;

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticketDto;
        }

        public async Task<TicketDto?> CreateTicket(TicketPostDto ticketPostDto, string userId)
        {
            var exist = await _repository.Any(t => t.EventId == ticketPostDto.EventId && t.UserId == userId);

            if (exist)
                return null;

            var evnt = await _eventRepository.GetById(ticketPostDto.EventId);

            if (evnt == null)
                return null;

            var quantityTickets = await _repository.Count(t => t.EventId == ticketPostDto.EventId);

            if (quantityTickets >= evnt.Capacity)
                return null;

            var ticket = new Ticket
            {
                EventId = ticketPostDto.EventId,
                UserId = userId
            };

            await _repository.Create(ticket);
            await _repository.Save();

            var newTicket = await _repository.GetTicketWithEvent(ticket.Id, userId);
            var ticketDto = _mapper.Map<TicketDto>(newTicket);

            return ticketDto;
        }

        public async Task<bool> DeleteTicket(int id, string userId)
        {
            var ticket = await _repository.GetById(id);

            if (ticket == null)
                return false;

            if (ticket.UserId != userId)
                return false;

            _repository.Delete(ticket);
            await _repository.Save();

            return true;
        }
    }
}