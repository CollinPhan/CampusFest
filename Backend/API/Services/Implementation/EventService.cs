using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.Exceptions;
using Backend.Infrastructures.Repositories.Interface;
using Backend.Utilities.Helpers;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Backend.API.Services.Implementation
{
    public class EventService : IEventService
    {
        private readonly IBaseRepository<Account> accountRepo;
        private readonly IBaseRepository<Event> eventRepo;
        private readonly IBaseRepository<Club> clubRepo;
        private readonly IBaseRepository<EventRegistration> registrationRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public EventService(IBaseRepository<Account> accountRepo, IBaseRepository<Event> eventRepo, IBaseRepository<Club> clubRepo, IBaseRepository<EventRegistration> registrationRepo, IMapper mapper)
        {
            this.accountRepo = accountRepo;
            this.eventRepo = eventRepo;
            this.clubRepo = clubRepo;
            this.registrationRepo = registrationRepo;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<EventDTO>> GetPaginated(int page, int page_size, string keyword = "", int? campus = null, Guid? club = null, string orderBy = "", bool isAcending = false)
        {
            Expression<Func<Event, bool>> filter = 
                x => x.Name.ToLower().Contains(keyword.ToLower())
                && campus != null ? x.Club.CampusId == campus : true
                && club != null ? x.ClubId == club : true;

            IList<string> includeProperties = new List<string> { "Club", "Club.Campus" };

            Expression<Func<Event, object>> order = null!;

            switch (orderBy)
            {
                case "name":
                    order = x => x.Name;
                    break;
                case "club":
                    order = x => x.Club.Name;
                    break;
                case "campus":
                    order = x => x.Club.Campus.Id;
                    break;
                default:
                    break;
            }

            var result = await eventRepo.GetPaginated(page, page_size, includeProperties, filter, order);

            return mapper.Map<IEnumerable<Event>, IEnumerable<EventDTO>>(result);
        }

        public async Task<IEnumerable<EventDTO>> GetPaginated(int page, int page_size, Expression<Func<EventDTO, bool>> predicate)
        {
            var result = await eventRepo.GetAll();

            return mapper.Map<IEnumerable<Event>, IEnumerable<EventDTO>>(result);
        }

        public async Task<EventDTO?> GetInformation(Guid eventId)
        {
            var target = await eventRepo.GetById(eventId);

            if (target == null)
            {
                return null;
            }

            return mapper.Map<Event,EventDTO>(target);
        }

        public async Task<Guid?> CreateEvent(EventDTO eventInformation)
        {
            var entity = mapper.Map<EventDTO, Event>(eventInformation);

            await eventRepo.Create(entity);

            return entity.Id;
        }

        public Task<Guid?> UpdateEvent(EventDTO eventInformation)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EventRegisterDTO>> GetEventRegisteredList(Guid eventId)
        {
            var targetEvent = await eventRepo.GetById(eventId);

            if (targetEvent == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Can not find the information about required envent",
                    "Event_Not_Found_Exception",
                    "Invalid",
                    "Event does not exist",
                    "Can not find event information for the requested event Id",
                    eventId);
                return null!;
            }

            var resultList = await registrationRepo.GetPaginated(1, int.MaxValue, null!, x => x.EventId == eventId);

            return mapper.Map<IEnumerable<EventRegistration>, IEnumerable<EventRegisterDTO>>(resultList);
        }

        public async Task<Guid?> DeleteEvent(Guid eventId)
        {

            var target = await eventRepo.GetById(eventId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Can not find the information about required envent",
                    "Event_Not_Found_Exception",
                    "Invalid",
                    "Event does not exist",
                    "Can not find event information for the requested event Id",
                    eventId);
                return null!;
            }

            await eventRepo.Remove(target);

            return target.Id;
        }

        public async Task<bool> ValidateEventInformation(EventDTO eventInformation)
        {
            if (eventInformation.EndDate < eventInformation.StartDate)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Event end date can not set before start date.",
                    "Event_Validation_Exception",
                    "Invalid",
                    "Invalid event end date",
                    "Event end date value can only be on the same or after the start date",
                    eventInformation.EndDate);
            }

            if (eventInformation.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Event end date can not set before today.",
                    "Event_Validation_Exception",
                    "Invalid",
                    "Invalid event end date",
                    "Event start day value can only be set on or after today",
                    eventInformation.EndDate);
            }

            if (eventInformation.EndTime < eventInformation.StartTime)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Event end time can not set before start time.",
                    "Event_Validation_Exception",
                    "Invalid",
                    "Invalid event end time",
                    "Event end date can only be on the same or after the start date",
                    eventInformation.EndDate);
            }

            var start = eventInformation.StartDate.ToDateTime(eventInformation.StartTime);
            var end = eventInformation.EndDate.ToDateTime(eventInformation.EndTime);

            Expression<Func<Event, bool>> current_active_event_filter = x =>
            ((x.StartDate >= start && x.StartDate <= end)
            || (x.EndDate >= start && x.EndDate <= end)) && x.Id != eventInformation.Id;

            var current_active_event_in_time_range = await eventRepo.GetPaginated(1, int.MaxValue, null!, current_active_event_filter);

            if (current_active_event_in_time_range.Count() > 0)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Event time range is invalid",
                    "Event_Validation_Exception",
                    "Invalid",
                    "Invalid event time range",
                    "There's already another event occured during the selected time frame, please choose another one.",
                    eventInformation.EndDate);
            }

            return true;
        }


        public async Task<IEnumerable<EventDTO>> GetEventListInTimeRange(DateTime start, DateTime end)
        {
            Expression<Func<Event, bool>> time_filter = x => x.StartDate >= start && x.EndDate <= end;

            var result = await eventRepo.GetPaginated(1, int.MaxValue, null!, time_filter);

            return mapper.Map<IEnumerable<Event>, IEnumerable<EventDTO>>(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    accountRepo.Dispose();
                    eventRepo.Dispose();
                    clubRepo.Dispose();
                    registrationRepo.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
