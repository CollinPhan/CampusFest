using AutoMapper;
using Backend.API.Services.Implementation;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Exceptions;
using Backend.Cores.ViewModels;
using Backend.Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Route("api/club")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IClubService clubService;
        private readonly ICampusService campusService;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public ClubController(IClubService clubService, ICampusService campusService, IAccountService accountService, ITokenService tokenService, IEmailService emailService, IMapper mapper)
        {
            this.clubService = clubService;
            this.campusService = campusService;
            this.accountService = accountService;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClubPublicViewModel>> GetClubInformation(Guid id)
        {
            var result = await clubService.GetClub(id);

            if (result == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "club does not exist",
                    error: "Club_Exception",
                    type: "NotFound",
                    summary: "Can not find club information",
                    detail: "Club information does not exist for the provided id",
                    value: id
                    );
                return null!;
            }

            return Ok(mapper.Map<ClubDTO, ClubPublicViewModel>(result));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubPublicViewModel>>> GetClubInformation([FromQuery]int page = 1, [FromQuery]int page_size = 10, [FromQuery] string keyword = "")
        {
            var result = await clubService.GetPaginated(page, page_size, keyword);

            return Ok(mapper.Map<IEnumerable<ClubDTO>, IEnumerable<ClubPublicViewModel>>(result));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateNewClubAccount([FromBody] ClubManagerAccountCreationModel clubAccountInfo)
        {
            AccountDTO account = mapper.Map<ClubManagerAccountCreationModel, AccountDTO>(clubAccountInfo);
            account.Role = "ClubEventOrganizer";

            await accountService.ValidateAccountRegisterInformation(account);

           ClubDTO club = mapper.Map<ClubManagerAccountCreationModel, ClubDTO>(clubAccountInfo);

            await clubService.ValidateClubRegisterInformation(club);
            account.Club = club.Id;

            await clubService.AddNewClub(club);

            await accountService.AddAccount(account);
            
            var token = await tokenService.CreateToken(account.Id, "verifyUserAccount", tokenService.CreateRandomToken(10), 1440);
            string emailSubject = "Account verification";

            string emailBody = System.IO.File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Utilities\\Html\\verification.html").Replace("[0]", clubAccountInfo.Fullname).Replace("[1]", token.TokenValue);

            await emailService.SendEmailAsync(account.Email, emailSubject, emailBody);

            return Created($"{HttpContext.Request.Host.Value}/{club.Id}", club.Id);

        }

        [HttpGet("{id}/staff")]
        public async Task<ActionResult<IEnumerable<AccountPublicViewModel>>> GetClubStaffs(Guid id)
        {
            List<string> includes = new List<string>() { "Role" };
            var result = await accountService.GetAccountPaginated(1, int.MaxValue, includes, x => x.ClubId == id);

            return Ok(mapper.Map<IEnumerable<AccountPublicViewModel>>(result));
        }

        [HttpPut]
        public async Task<ActionResult<ClubUpdateModel>> UpdateClubInformation(ClubUpdateModel clubInformation)
        {
            await clubService.UpdateClub(mapper.Map<ClubUpdateModel, ClubDTO>(clubInformation));

            return Ok(clubInformation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(Guid id)
        {
            await clubService.DeleteClub(id);

            return Ok(id);
        }
    }
}
