using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Exceptions;
using Backend.Cores.ViewModels;
using Backend.Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ICampusService campusService;
        private readonly IMapper mapper;

        public CampusController(IAccountService accountService, ICampusService campusService, IMapper mapper)
        {
            this.accountService = accountService;
            this.campusService = campusService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampusPublicViewModel>>> GetPaginated([FromQuery] int page = 1, [FromQuery] int page_size = 10, [FromQuery] string keyword = "")
        {
            var result = await campusService.GetCampusPaginated(page, page_size, keyword);

            return Ok(mapper.Map<IEnumerable<CampusDTO>, IEnumerable<CampusPublicViewModel>>(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CampusPublicViewModel>> GetCampusInformation(int id)
        {
            var target = await campusService.GetCampus(id);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Can not find information for campus with provided Id",
                    "Campus_Not_Found_Exception",
                    "NotFound",
                    "Campus is not existed",
                    "Campus information can not be found using the provied information",
                    id);
                return null!;
            }

            return Ok(mapper.Map<CampusDTO, CampusPublicViewModel>(target));
        }

        [HttpPost]
        public async Task<ActionResult> AddCampusInformation(CampusCreationModel campusInfo)
        {
            var dto = mapper.Map<CampusCreationModel, CampusDTO>(campusInfo);

            await campusService.AddCampus(dto);

            return Created();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> GetCampusInformation(int id, CampusUpdateModel campusInfo)
        {
            var dto = mapper.Map<CampusUpdateModel, CampusDTO>(campusInfo);

            dto.Id = id;

            await campusService.UpdateCampus(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCampus(int id)
        {
            await campusService.DeleteCampus(id);

            return Ok();
        }
    }
}
