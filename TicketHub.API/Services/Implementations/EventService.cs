using AutoMapper;
using TicketHub.API.DTOs.Event;
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

        public EventService(IEventRepository repository, IMapper mapper, IRepository<Category> categoryRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedList<EventDto>> GetEvents(PaginationParams pParams)
        {
            var pEvents = await _repository.GetEventsWithCategory(pParams);

            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(pEvents.Items);

            return new PagedList<EventDto>(eventsDto, pEvents.TotalCount, pEvents.CurrentPage, pEvents.PageSize);
        }

        public async Task<EventDto?> GetEvent(int id)
        {
            var evnt = await _repository.GetEventByIdWithCategory(id);

            if (evnt == null)
                return null;

            var eventDto = _mapper.Map<EventDto>(evnt);

            return eventDto;
        }

        public async Task<EventDto?> CreateEvent(EventPostDto eventPostDto)
        {
            var exist = await _repository.Any(e =>
                e.Name!.ToUpper() == eventPostDto.Name!.ToUpper() && e.Date == eventPostDto.Date);

            if (exist)
                return null;

            var validCategory = await _categoryRepository.Any(c => c.Id == eventPostDto.CategoryId);

            if (!validCategory)
                return null;

            var evnt = _mapper.Map<Event>(eventPostDto);

            await _repository.Create(evnt);
            await _repository.Save();

            var newEvent = await _repository.GetById(evnt.Id);
            var eventDto = _mapper.Map<EventDto>(newEvent);

            return eventDto;
        }

        public async Task<bool> UpdateEvent(EventPutDto eventPutDto)
        {
            var validCategory = await _categoryRepository.Any(c => c.Id == eventPutDto.CategoryId);

            if (!validCategory)
                return false;

            var exist = await _repository.Any(e =>
                e.Name!.ToUpper() == eventPutDto.Name!.ToUpper() &&
                e.Date == eventPutDto.Date &&
                e.Id != eventPutDto.Id);

            if (exist)
                return false;

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
            var evnt = await _repository.GetById(id);

            if (evnt == null)
                return false;

            _repository.Delete(evnt);
            await _repository.Save();

            return true;
        }
    }
}