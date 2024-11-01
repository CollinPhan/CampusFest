using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public RoleController(IAccountService accountService, IMapper mapper)
        {
            this.accountService = accountService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleViewModel>>> GetAllSystemRole()
        {
            var dtoList = await accountService.GetRoles();
            
            return Ok(mapper.Map<IEnumerable<RoleDTO>, IEnumerable<RoleViewModel>>(dtoList));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewRole([FromBody]RoleCreationModel roleInformation)
        {
            var dto = mapper.Map<RoleCreationModel, RoleDTO>(roleInformation);
            await accountService.AddRole(dto);

            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await accountService.RemoveRole(id);

            return NoContent();
        }
    }
}
