using Backend.Infrastructures.Data.DTO;
using System.Linq.Expressions;

namespace Backend.API.Services.Interface
{
    public interface ITokenService: IDisposable
    {
        Task CreateToken(Guid userId, string reason, string? tokenValue, int duration);

        Task DeleteToken(Guid tokenId);

        Task DeleteToken(string tokenValue);

        TokenDTO GetLatestToken(Guid userId, string reason);

        string CreateAccessToken(Dictionary<string, string> data, int duration);
        string CreateRefreshToken(Dictionary<string, string> data, int duration);

        IEnumerable<TokenDTO> GetAllTokenForUser(Guid userId);

        string CreateJwtToken(Dictionary<string, string> data, int duration);

        string CreateBase64Token(Dictionary<string, string> data, int duration);

        string CreateCustomToken(Dictionary<string, string> data, Expression<Func<Dictionary<string, string>, string>> function);

        Task<Dictionary<string, string>> DecodeJwtToken(string token);

        Dictionary<string, string> DecodeBase64Token(string token);

        Dictionary<string, string> DecodeCustomToken(string token);

        bool IsValidJwtToken(string token);

        bool IsValidBase64Token(string token);
    }
}
