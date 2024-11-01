using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.Commons;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.Exceptions;
using Backend.Infrastructures.Repositories.Interface;
using Backend.Utilities.Helpers;
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
        private readonly IBaseRepository<Token> tokenRepo;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private bool disposedValue;

        Dictionary<string, object> ExceptionData = new Dictionary<string, object>
        {
            { "error",  "Token_Exception"},
            { "detail", "Unknown_Exception"},
            { "type", "Unknown"},
            { "value", null!},
        };


        public TokenService(IBaseRepository<Token> tokenRepository, IMapper mapper, IConfiguration configuration)
        {
            this.tokenRepo = tokenRepository;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public string CreateAccessToken(Dictionary<string, string> data, int duration = 5)
        {
            return CreateJwtToken(data, duration);
        }

        public string CreateRefreshToken(Dictionary<string, string> data, int duration = 10)
        {
            return CreateBase64Token(data, duration);
        }


        public string CreateBase64Token(Dictionary<string, string> data, int duration = 5)
        {
            // Header section
            DateTime creation = DateTime.UtcNow;
            DateTime expiration = creation.AddMinutes(duration);
            string timeFormat = "yyyy-MM-ddTHH:mm:ss.000Z";
            string headerSection = $"{{creation:{creation.ToString(timeFormat)},expiration:{expiration.ToString(timeFormat)}}}";

            // Signature section
            string signature = $"{creation.ToString(timeFormat)}";

            // Body section
            string bodySection = "{";

            if (data != null)
            {
                foreach (KeyValuePair<string, string> x in data)
                {
                    bodySection += $"{x.Key}:{x.Value},";
                }
            }

            bodySection = bodySection.Substring(0, bodySection.Length - 1) + "}";

            var byteSignature = Rfc2898DeriveBytes.Pbkdf2(signature, Encoding.UTF8.GetBytes(expiration.ToString(timeFormat)), 32, HashAlgorithmName.SHA256, 32);

            Console.WriteLine(bodySection);

            return $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(headerSection))}.{Convert.ToBase64String(Encoding.UTF8.GetBytes(bodySection))}.{Convert.ToBase64String(byteSignature)}";
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

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Issuer"], claims, expires: DateTime.UtcNow.AddMinutes(duration), signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRandomToken(int length = 10)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!=_-@#$%";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);

            return finalString;
        }

        public Dictionary<string, string> DecodeBase64Token(string token)
        {
            List<string> tokenParts = token.Split(".").ToList();

            // Token verification and validation

            // token format validation
            if (tokenParts.Count != 3)
            {
                var exception = new Exception("The given token is not valid");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Refresh_Token_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", token);

                throw exception;
            }

            // token signature validation
            string header = Encoding.UTF8.GetString(Convert.FromBase64String(tokenParts[0]));
            header = header.Remove(header.Length - 1, 1).Remove(0, 1); // Remove parenthesis.

            Dictionary<string, string> headerData = new Dictionary<string, string>();

            foreach (string item in header.Split(","))
            {
                var entry = item.Split(":", 2);
                headerData.Add(entry[0], entry[1]);
            }

            var byteSignature = Rfc2898DeriveBytes.Pbkdf2($"{headerData["creation"]}", Encoding.UTF8.GetBytes(headerData["expiration"]), 32, HashAlgorithmName.SHA256, 32);

            if (Convert.ToBase64String(byteSignature) != tokenParts[2])
            {
                var exception = new Exception("The given token signature is not valid");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Refresh_Token_Invalid");
                exception.Data.Add("type", "Unauthorized");
                exception.Data.Add("value", token);

                throw exception;
            }

            if (DateTime.Parse(headerData["expiration"]) < DateTime.UtcNow)
            {
                var exception = new Exception("The given token is expired");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Refresh_Token_Expired");
                exception.Data.Add("type", "Unauthorized");
                exception.Data.Add("value", token);

                throw exception;
            }

            // Actual decoding for data part
            string body = Encoding.UTF8.GetString(Convert.FromBase64String(tokenParts[1]));

            body = body.Remove(body.Length - 1, 1).Remove(0, 1); // Remove parenthesis.

            Dictionary<string, string> bodyData = new Dictionary<string, string>();

            foreach (string item in body.Split(","))
            {
                var entry = item.Split(":");
                bodyData.Add(entry[0], entry[1]);
            }

            return bodyData;
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

            var ValidationResult = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParams);

            if (!ValidationResult.IsValid)
            {
                var exception = new Exception("Security Token Is Not Valid");

                // Add Data to Exception
                exception.Data.Add("statusCode", 400);
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


        public async Task<TokenDTO> CreateToken(Guid userId, string reason, string? tokenValue, int duration)
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

                token.Value = CreateRandomToken(12);
            }
            
            return mapper.Map<Token, TokenDTO>(await tokenRepo.Create(token));
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
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", tokenId);

                throw exception;
            }

            await tokenRepo.Remove(target);
        }

        public async Task DeleteToken(string tokenValue)
        {
            var target = (await tokenRepo.GetPaginated(1, 1, null!, x => x.Value == tokenValue, x => x.Value)).FirstOrDefault();

            if (target == null)
            {
                var exception = new Exception("Token information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Not_Found");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", tokenValue);

                throw exception;
            }

            await tokenRepo.Remove(target);
        }

        public IEnumerable<TokenDTO> GetAllTokenForUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDTO> GetLatestToken(Guid userId, string reason)
        {
            Expression<Func<Token, bool>> filterExpression = x => x.ValidAccount == userId && x.Reason == reason;
            Expression<Func<Token, object>> sortExpression = x => x.ExpirationDate;

            var tokenEntry = (await tokenRepo.GetPaginated(1, 1, null!, filterExpression, sortExpression)).FirstOrDefault();

            if (tokenEntry == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Account data is not found",
                    error: "Account_Exception",
                    type: "Invalid",
                    summary: "The account data is not found",
                    detail: "The account information ",
                    value: userId);
            }

            return mapper.Map<Token,TokenDTO>(tokenEntry!);
        }
        
        public async Task<TokenDTO> GetToken(Guid tokenEntry)
        {
            var result = await tokenRepo.GetById(tokenEntry);

            if (result == null)
            {
                var exception = new Exception();

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Not_Existed");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", tokenEntry);
                throw exception;
            }

            return mapper.Map<Token,TokenDTO>(result);
        }

        public async Task<Guid> GetValidUserForToken(string tokenValue)
        {
            var token = await tokenRepo.FindFirstMatch("value", tokenValue);

            if (token == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Token is not found",
                    error: "Token_Exception",
                    type: "Invalid",
                    summary: "The token value could not be found.",
                    detail: "This is likely because the given is wrong or the token is deleted.",
                    value: tokenValue);
            }

            return token!.ValidAccount;
        }

        public async Task<bool> VerifyToken(Guid userId, string tokenValue, string reason)
        {
            Expression<Func<Token, bool>> filterExpression = x => x.ValidAccount == userId && x.Reason == reason && x.Value == tokenValue;
            Expression<Func<Token, object>> sortExpression = x => x.ExpirationDate;

            var tokenEntry = (await tokenRepo.GetPaginated(1, 1, null!, filterExpression, sortExpression)).FirstOrDefault();

            if (tokenEntry == null)
            {
                var exception = new Exception();

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", userId);

                throw exception;
            }

            if (tokenEntry.ExpirationDate < DateTime.UtcNow)
            {
                var exception = new Exception();

                // Add Data to Exception
                exception.Data.Add("error", "Token_Exception");
                exception.Data.Add("detail", "Token_Expired");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", userId);

                throw exception;
            }

            return true;
        }

        public async Task<Guid> VerifyToken(string tokenValue, string reason)
        {
            Expression<Func<Token, bool>> filterExpression = x => x.Reason == reason && x.Value == tokenValue;
            Expression<Func<Token, object>> sortExpression = x => x.ExpirationDate;

            var tokenEntry = (await tokenRepo.GetPaginated(1, 1, null!, filterExpression, sortExpression)).FirstOrDefault();

            if (tokenEntry == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Token is not found",
                    error: "Token_Exception",
                    type: "Invalid",
                    summary: "The token value could not be found.",
                    detail: "This is likely because the given is wrong or the token is deleted.",
                    value: tokenValue);
            }

            if (tokenEntry!.ExpirationDate < DateTime.UtcNow)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Token is not found",
                    error: "Token_Exception",
                    type: "Invalid",
                    summary: "The token is expired.",
                    detail: $"The token is expired at {tokenEntry!.ExpirationDate}",
                    value: tokenValue);
            }

            return tokenEntry.ValidAccount;
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
