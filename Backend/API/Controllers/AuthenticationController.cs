using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [Authorize]
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AuthenticationController(IAccountService accountService, ITokenService tokenService, IMapper mapper)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public ActionResult RefreshAccessToken([FromBody] string oldRefreshToken)
        {
            var data = tokenService.DecodeBase64Token(oldRefreshToken);

            var accessToken = tokenService.CreateAccessToken(data, 5);

            var refreshToken = tokenService.CreateRefreshToken(data, 10);

            return Ok(new { accessToken, refreshToken });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateUser([FromBody] AccountAuthorizationModel authorizationInfo)
        {
            AccountDTO account = await accountService.GetAccountInformation(authorizationInfo.Username, authorizationInfo.Password);

            Dictionary<string, string> tokenInformation = new Dictionary<string, string>();

            tokenInformation.Add("user", account.Id.ToString());
            tokenInformation.Add("roles", String.Join(",", account.Role));
            tokenInformation.Add("verified", account.IsVerified ? "Yes" : "No");


            var accessToken = tokenService.CreateJwtToken(tokenInformation, 5);
            var refreshToken = tokenService.CreateBase64Token(tokenInformation, 5);

            return Ok(new { accessToken, refreshToken });
        }
    }
}
