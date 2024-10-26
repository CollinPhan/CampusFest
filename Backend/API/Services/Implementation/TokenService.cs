using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.Entities;
using Backend.Infrastructures.Data.DTO;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.API.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository tokenRepo;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private bool disposedValue;

        public TokenService(ITokenRepository tokenRepository, IMapper mapper, IConfiguration configuration)
        {
            this.tokenRepo = tokenRepository;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public string CreateAccessToken(Dictionary<string, string> data, int duration = 5)
        {
            return CreateJwtToken(data, duration);
        }

        public string CreateBase64Token(Dictionary<string, string> data, int duration = 5)
        {
            // Header section
            DateTime creation = DateTime.UtcNow;
            DateTime expiration = creation.AddMinutes(duration);
            string timeFormat = "yyyy-MM-ddTHH:mm:ss.000Z";
            string Rng = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();
            string headerSection = $"{{RNGKey:{Rng},creation:{creation.ToString(timeFormat)},expiration:{expiration.ToString(timeFormat)}}}";

            // Signature sectino
            string signature = $"{creation.ToString(timeFormat)}";

            // Body section
            string bodySection = "{";

            foreach (KeyValuePair<string, string> x in data)
            {
                bodySection += $"{x.Key}:{x.Value},";
                signature += x.Value;
            }
            
            bodySection = bodySection.Substring(0, bodySection.Length - 1) + "}";

            var byteSignature = Rfc2898DeriveBytes.Pbkdf2(signature, Encoding.UTF8.GetBytes(expiration.ToShortDateString()), 32, HashAlgorithmName.SHA256, 32);

            return $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(headerSection))}.{Convert.ToBase64String(Encoding.UTF8.GetBytes(bodySection))}.{Convert.ToBase64String(byteSignature)}";
        }


        public string CreateCustomToken(Dictionary<string, string> data, Expression<Func<Dictionary<string, string>, string>> function)
        {
            throw new NotImplementedException();
        }

        public string CreateJwtToken(Dictionary<string, string>data, int duration = 5)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>();


            foreach (KeyValuePair<string, string> set in data)
            {
                claims.Add(new Claim(set.Key, set.Value));
            }

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Issuer"], expires: DateTime.UtcNow.AddMinutes(duration), signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken(Dictionary<string, string> data, int duration = 10)
        {
            return CreateBase64Token(data, duration);
        }

        public async Task CreateToken(Guid userId, string reason, string? tokenValue, int duration)
        {
            Token token = new Token
            {
                ValidAccount = userId,
                Value = tokenValue!,
                Reason = reason,
                ExpirationDate = DateTime.UtcNow.AddMinutes(duration)
            };

            // Data appending in case token value was not passed
            if (tokenValue == null)
            {
                var tokenData = new Dictionary<string, string>
                {
                    { "user", userId.ToString() },
                    { "reason", reason },
                };

                token.Value = CreateBase64Token(tokenData, duration);
            }
            
            await tokenRepo.Create(token);
        }

        public Dictionary<string, string> DecodeBase64Token(string token)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> DecodeCustomToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, string>> DecodeJwtToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            TokenValidationParameters validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidAudience = configuration["Jwt:Issuer"],
                ValidIssuer = configuration["Jwt:Issuer"],
                IssuerSigningKey = key
            };

            var ValidationResult =await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParams);

            if (!ValidationResult.IsValid)
            {
                var exception = new Exception("Security Token Is Not Valid");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Invalid");
                exception.Data.Add("value", token);

                throw exception;
            }

            Dictionary<string, string> data = new Dictionary<string, string>();

            var tokenObject = new JwtSecurityTokenHandler().ReadJwtToken(token);

            foreach (var claim in tokenObject.Claims)
            {
                data.Add(claim.Type, claim.Value);
            }

            return data;
        }

        public async Task DeleteToken(Guid tokenId)
        {
            var target = await tokenRepo.GetById(tokenId);

            if (target == null)
            {
                var exception = new Exception("Token information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Not_Found");
                exception.Data.Add("value", tokenId);

                throw exception;
            }

            await tokenRepo.Remove(target);
        }

        public async Task DeleteToken(string tokenValue)
        {
            var target = (await tokenRepo.GetPaginated(1, 1, x => x.Value == tokenValue, "Value")).FirstOrDefault();

            if (target == null)
            {
                var exception = new Exception("Token information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Not_Found");
                exception.Data.Add("value", tokenValue);

                throw exception;
            }

            await tokenRepo.Remove(target);
        }

        public IEnumerable<TokenDTO> GetAllTokenForUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public TokenDTO GetLatestToken(Guid userId, string reason)
        {
            throw new NotImplementedException();
        }

        public bool IsValidBase64Token(string token)
        {
            throw new NotImplementedException();
        }

        public bool IsValidJwtToken(string token)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    tokenRepo.Dispose();
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
