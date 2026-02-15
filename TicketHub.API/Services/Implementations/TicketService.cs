using System.Data;
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
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository repository,
            IMapper mapper,
            IRepository<Event> eventRepository,
            ILogger<TicketService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _logger = logger;
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
            _logger.LogInformation("Obteniendo el Ticket {id} para el usuario {user}", id, userId);

            var ticket = await _repository.GetTicketWithEvent(id, userId);

            if (ticket == null)
            {
                _logger.LogWarning("No se encontró el Ticket {id} del usuario {user}", id, userId);
                return null;
            }

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticketDto;
        }

        public async Task<TicketDto?> CreateTicket(TicketPostDto ticketPostDto, string userId)
        {
            _logger.LogInformation("Creando Ticket para el evento {id}", ticketPostDto.EventId);

            var exist = await _repository.Any(t => t.EventId == ticketPostDto.EventId && t.UserId == userId);

            if (exist)
            {
                _logger.LogWarning("No se puede crear el Ticket para el evento {eId} porque ya existe el Ticket",
                    ticketPostDto.EventId);
                return null;
            }


            var evnt = await _eventRepository.GetById(ticketPostDto.EventId);

            if (evnt == null)
            {
                _logger.LogWarning("No se puede crear Ticket para el evento {eId} porque este no existe",
                    ticketPostDto.EventId);
                return null;
            }


            if (evnt.SoldTickets >= evnt.Capacity)
            {
                _logger.LogWarning("No se puede crear Ticket para el evento {eId} porque está lleno", evnt.Id);
                return null;
            }


            var ticket = new Ticket
            {
                EventId = ticketPostDto.EventId,
                UserId = userId
            };

            evnt.SoldTickets++;

            try
            {
                await _repository.Create(ticket);
                await _repository.Save();
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error de concurrencia al querer comprar una entrada para en evento {id}",
                    ticketPostDto.EventId);
                throw new Exception("Reintente mas tarde...");
            }

            var newTicket = await _repository.GetTicketWithEvent(ticket.Id, userId);
            var ticketDto = _mapper.Map<TicketDto>(newTicket);

            return ticketDto;
        }

        public async Task<bool> DeleteTicket(int id, string userId)
        {
            _logger.LogInformation("Eliminando Ticket {id} del usuario {user}", id, userId);

            var ticket = await _repository.GetById(id);

            if (ticket == null)
            {
                _logger.LogWarning("No se puede eliminar el Ticket {id} porque no existe", id);
                return false;
            }

            if (ticket.UserId != userId)
            {
                _logger.LogWarning("El usuario {user} intento eliminar el Ticket de {userProp}", userId, ticket.UserId);
                return false;
            }

            _repository.Delete(ticket);
            await _repository.Save();

            return true;
        }
    }
}