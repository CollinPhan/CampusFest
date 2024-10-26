using Backend.Cores.Entities;
using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Interface;

namespace Backend.Infrastructures.Repositories.Implementation
{
    public class TokenRepository : BaseRepositoy<Token>, ITokenRepository
    {
        public TokenRepository(CampusFestDbContext context) : base(context) { }
    }
}
