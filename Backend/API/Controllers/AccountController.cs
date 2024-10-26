using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.ViewModels;
using Backend.Infrastructures.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(IAccountService accountService, ITokenService tokenService, IMapper mapper)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RefreshAccessToken([FromHeader] string oldRefreshToken)
        {
            var data = tokenService.DecodeBase64Token(oldRefreshToken);

            var accessToken = tokenService.CreateAccessToken(data, 5);

            var refreshToken = tokenService.CreateRefreshToken(data, 10);

            return Ok(new { accessToken, refreshToken });
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<AccountPublicViewModel>>> GetAllAccountPublicInformation(int page = 1, int page_size = 10, string keyword = "", string sortBy = "Username", bool onlyVerified = false, bool includeDeleted = false)
        {
            return Ok(await accountService.GetAccountPaginated(page: page, page_size: page_size, username: keyword, sortby: sortBy, IncludeDeleted: includeDeleted, OnlyVerified: onlyVerified));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountPublicViewModel>> GetAccountPublicInformation(Guid id)
        {
            return Ok(await accountService.GetAccountInformation(id));
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser([FromBody] AccountAuthorizationViewModel authorizationInfo)
        {
            AccountDTO account = await accountService.GetAccountInformation(authorizationInfo.Username, authorizationInfo.Password);

            Dictionary<string, string> tokenInformation = new Dictionary<string, string>();

            tokenInformation.Add("user", account.Id.ToString());
            tokenInformation.Add("roles", String.Join(",", account.Roles));
            tokenInformation.Add("verified", account.IsVerified ? "Yes" : "No");


            var accessToken = tokenService.CreateJwtToken(tokenInformation, 5);
            var refreshToken = tokenService.CreateBase64Token(tokenInformation, 5);

            return Ok(new { accessToken, refreshToken });
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> CreateNewUser([FromBody] AccountCreationModel accountInfo)
        {
            await accountService.AddAccount(mapper.Map<AccountCreationModel, AccountDTO>(accountInfo));

            return Created();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountInformation(AccountUpdateModel accountInfo)
        {
            await accountService.UpdateAccount(mapper.Map<AccountUpdateModel, AccountDTO>(accountInfo));

            return NoContent();
        }
    }
}
