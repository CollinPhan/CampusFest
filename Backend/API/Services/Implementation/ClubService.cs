using Backend.API.Services.Interface;
using Backend.Cores.Entities;
using Backend.Infrastructures.Repositories.Interface;

namespace Backend.API.Services.Implementation
{
    public class ClubService : IClubService
    {
        private readonly IBaseRepository<Club> clubRepo;
        private readonly IBaseRepository<Campus> campusRepo;
        private readonly IBaseRepository<Account> accountRepo;
        private bool disposedValue;

        public ClubService(IBaseRepository<Club> clubRepo, IBaseRepository<Campus> campusRepo, IBaseRepository<Account> accountRepo)
        {
            this.clubRepo = clubRepo;
            this.campusRepo = campusRepo;
            this.accountRepo = accountRepo;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    clubRepo.Dispose();
                    campusRepo.Dispose();
                    accountRepo.Dispose();
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
