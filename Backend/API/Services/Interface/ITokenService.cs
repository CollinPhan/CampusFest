using Backend.Infrastructures.Data.DTO;
using System.Linq.Expressions;

namespace Backend.API.Services.Interface
{
    public interface ITokenService: IDisposable
    {
        Task<TokenDTO> CreateToken(Guid userId, string reason, string? tokenValue, int duration);

        Task DeleteToken(Guid tokenId);

        Task DeleteToken(string tokenValue);

        Task<TokenDTO> GetToken(Guid tokenEntry);

        Task<bool> VerifyToken(Guid userId, string tokenValue, string reason);

        Task<Guid> VerifyToken(string tokenValue, string reason);


        Task<TokenDTO> GetLatestToken(Guid userId, string reason);

        Task<Guid> GetValidUserForToken(string tokenValue);

        string CreateAccessToken(Dictionary<string, string> data, int duration);

        string CreateRefreshToken(Dictionary<string, string> data, int duration);

        IEnumerable<TokenDTO> GetAllTokenForUser(Guid userId);

        string CreateJwtToken(Dictionary<string, string> data, int duration);

        string CreateBase64Token(Dictionary<string, string> data, int duration);

        string CreateRandomToken(int length);

        Task<Dictionary<string, string>> DecodeJwtToken(string token);

        Dictionary<string, string> DecodeBase64Token(string token);

        bool IsValidJwtToken(string token);

        bool IsValidBase64Token(string token);
    }
}
