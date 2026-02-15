using AutoMapper;
using TicketHub.API.DTOs.Event;
using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Models;
using TicketHub.API.Pagination;
using TicketHub.API.Repository;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.API.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository repository,
            IMapper mapper,
            IRepository<Category> categoryRepository,
            ILogger<EventService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<PagedList<EventDto>> GetEvents(PaginationParams pParams)
        {
            var pEvents = await _repository.GetEventsWithCategory(pParams);

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(pEvents.Items);

            return new PagedList<EventDto>(eventsDto, pEvents.TotalCount, pEvents.CurrentPage, pEvents.PageSize);
        }

        public async Task<EventDto?> GetEvent(int id)
        {
            _logger.LogInformation("Obteniendo el evento {id}", id);

            var evnt = await _repository.GetEventByIdWithCategory(id);

            if (evnt == null)
                return null;

            var eventDto = _mapper.Map<EventDto>(evnt);

            return eventDto;
        }

        public async Task<EventDto?> CreateEvent(EventPostDto eventPostDto)
        {
            _logger.LogInformation("Creando evento con nombre: {name}", eventPostDto.Name);

            var exist = await _repository.Any(e =>
                e.Name!.ToUpper() == eventPostDto.Name!.ToUpper() && e.Date.Hour == eventPostDto.Date.Hour);

            if (exist)
            {
                _logger.LogWarning("El evento con nombre {name} ya existe en esa fecha", eventPostDto.Name);
                return null;
            }

            var validCategory = await _categoryRepository.Any(c => c.Id == eventPostDto.CategoryId);

            if (!validCategory)
            {
                _logger.LogWarning("La categoría con id {id} no existe para crear el evento {name}",
                    eventPostDto.CategoryId, eventPostDto.Name);
                return null;
            }

            var evnt = _mapper.Map<Event>(eventPostDto);

            await _repository.Create(evnt);
            await _repository.Save();

            var newEvent = await _repository.GetEventByIdWithCategory(evnt.Id);
            var eventDto = _mapper.Map<EventDto>(newEvent);

            return eventDto;
        }

        public async Task<bool> UpdateEvent(EventPutDto eventPutDto)
        {
            _logger.LogInformation("Actualizando el evento {id}", eventPutDto.Id);

            var validCategory = await _categoryRepository.Any(c => c.Id == eventPutDto.CategoryId);

            if (!validCategory)
            {
                _logger.LogWarning("La categoría {cId} no existe para actualizar el evento {id}",
                    eventPutDto.CategoryId, eventPutDto.Id);
                return false;
            }


            var exist = await _repository.Any(e =>
                e.Name!.ToUpper() == eventPutDto.Name!.ToUpper() &&
                e.Date == eventPutDto.Date &&
                e.Id != eventPutDto.Id);

            if (exist)
            {
                _logger.LogWarning("No se puede actualizar el evento {id} porque ya existe otro evento con el nombre {name} en la misma fecha",
                    eventPutDto.Id, eventPutDto.Name);
                return false;
            }


            var evnt = await _repository.GetById(eventPutDto.Id);

            if (evnt == null)
                return false;

            _mapper.Map(eventPutDto, evnt);
            _repository.Update(evnt);
            await _repository.Save();

            return true;
        }

        public async Task<bool> DeleteEvent(int id)
        {
            _logger.LogInformation("Eliminando el evento {id}", id);

            var evnt = await _repository.GetById(id);

            if (evnt == null)
                return false;

            _repository.Delete(evnt);
            await _repository.Save();

            return true;
        }
    }
}