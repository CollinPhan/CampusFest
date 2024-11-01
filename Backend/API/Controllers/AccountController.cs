using System.Reflection;
using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Exceptions;
using Backend.Cores.ViewModels;
using Backend.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    //[Authorize]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public AccountController(IAccountService accountService, ITokenService tokenService, IMapper mapper, IEmailService emailService)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AccountPublicViewModel>>> GetAllAccountPublicInformation(int page = 1, int page_size = 10, string keyword = "", string sortBy = "Username", bool onlyVerified = false, bool includeDeleted = false)
        {
            var result = mapper.Map<IEnumerable<AccountDTO>, IEnumerable<AccountPublicViewModel>>(await accountService.GetAccountPaginated(page: page, page_size: page_size, username: keyword, sortby: sortBy, IncludeDeleted: includeDeleted, OnlyVerified: onlyVerified));
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<AccountPublicViewModel>> GetAccountPublicInformation()
        {
            string token = Request.Headers.Authorization.First<string>();

            Dictionary<string,string> data = await tokenService.DecodeJwtToken(token.Split(" ")[1]); // Only getting the token without the "Bearer" part.

            Console.WriteLine(string.Join(" ", data.Keys));

            return Ok(await accountService.GetAccountInformation(Guid.Parse(data["user"])));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountPublicViewModel>> GetAccountPublicInformation(Guid id)
        {
            return Ok(await accountService.GetAccountInformation(id));
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> CreateNewUser([FromBody] AccountCreationModel accountInfo)
        {
            var account = mapper.Map<AccountCreationModel, AccountDTO>(accountInfo);
            account.Role = "Visitor";

            await accountService.AddAccount(account);
            
            var token = await tokenService.CreateToken(account.Id, "verifyUserAccount", tokenService.CreateRandomToken(10), 1440);

            string emailSubject = "Account verification";
            
            string emailBody = System.IO.File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Utilities\\Html\\verification.html").Replace("[0]", token.TokenValue);

            await emailService.SendEmailAsync(account.Email, emailSubject, emailBody);

            return Created();
        }

        [AllowAnonymous]
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyUserAccount([FromQuery] string token)
        {
            Guid userId = await tokenService.VerifyToken(token, "verifyUserAccount");   
            await tokenService.DeleteToken(token);
            await accountService.UpdateVerificationStatus(userId, true);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{userId}/verify-request")]
        public async Task<IActionResult> RequestVerifyUserAccount(Guid userId)
        {
            var account = await accountService.GetAccountInformation(userId);

            if (account == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Can not find account information with given id",
                    "Account_Verify_Request_Exception",
                    "Invalid",
                    "Accountt information not found",
                    "The given id does not match for any existing account",
                    userId);
                return null!;
            }

            if ( account.IsVerified)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   "Account has already been verified before",
                   "Account_Verify_Request_Exception",
                   "Unauthorized",
                   "Account has been verified",
                   "The target account has already been verified",
                   userId);
                return null!;
            }

            var token = await tokenService.CreateToken(account.Id, "verifyUserAccount", tokenService.CreateRandomToken(10), 1440);

            string emailSubject = "Account verification";
            
            string emailBody = System.IO.File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Utilities\\Html\\verification.html").Replace("[0]", token.TokenValue);

            await emailService.SendEmailAsync(account.Email, emailSubject, emailBody);

            return NoContent();
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
