using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Exceptions;
using Backend.Cores.ViewModels;
using Backend.Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Backend.API.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService eventService;
        private readonly IClubService clubService;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public EventController(IEventService eventService, IClubService clubService, IAccountService accountService, IMapper mapper)
        {
            this.eventService = eventService;
            this.clubService = clubService;
            this.accountService = accountService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventViewModel>>> GetEventPaginated([FromQuery] int page = 1, [FromQuery] int page_size = 10, [FromQuery] Guid? club = null, int? campus = null, long? startTime = null, long? endTime = null)
        {
            Expression<Func<EventDTO, bool>> filter = x => 
            club != null ? x.Club == club : true 
            && campus != null ? x.Campus == campus : true 
            && startTime != null ? (long)(x.StartDate.ToDateTime(x.StartTime) - DateTime.UtcNow).TotalSeconds >= startTime : true
            && endTime != null ? (long)(x.EndDate.ToDateTime(x.EndTime) - DateTime.UtcNow).TotalSeconds <= endTime : true; 

            var eventList = await eventService.GetPaginated(page, page_size, filter);

            return Ok(mapper.Map<IEnumerable<EventDTO>, IEnumerable<EventViewModel>>(eventList));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventViewModel>> GetEventInformation(Guid id)
        {
            var result = await eventService.GetInformation(id);

            if (result == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Can not find event information with the provided Id",
                    "Event_Not_Found_Exception",
                    "NotFuund",
                    "Event not found",
                    "Event information does not found with the provided Id",
                    id);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateEvent(EventCreationModel eventInfo)
        {
            var eventDto = mapper.Map<EventCreationModel, EventDTO>(eventInfo);

            if (!await eventService.ValidateEventInformation(eventDto))
            {
                return BadRequest();
            }

            return Ok(await eventService.CreateEvent(eventDto));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEvent(EventUpdateModel eventInfo)
        {
            var DTO = mapper.Map<EventUpdateModel, EventDTO>(eventInfo);

            if(!await eventService.ValidateEventInformation(DTO))
            {
                return BadRequest();
            }

            await eventService.UpdateEvent(DTO);

            return NoContent();
        }
    }
}
