using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.Entities;
using Backend.Cores.ViewModels;
using Backend.Infrastructures.Data.DTO;
using Backend.Infrastructures.Repositories.Interface;
using Backend.Utilities.Helpers;
using Microsoft.Identity.Client;
using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Backend.API.Services.Implementation
{
    public class AccountService: IAccountService
    {
        private readonly IAccountRepository accountRepo;
        private readonly IRoleRepository roleRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public AccountService(IAccountRepository accountRepo, IRoleRepository roleRepository, IMapper mapper)
        {
            this.accountRepo = accountRepo;
            this.roleRepo = roleRepository;
            this.mapper = mapper;
        }

        public async Task AddAccount(AccountDTO accountInfo)
        {
            Exception exception = null!;

            // Data validation for account creation

            if (await accountRepo.IsExist("Username", accountInfo.Username))
            {
                exception = new Exception("Username has been taken");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Username_Taken");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountInfo.Username);
            }

            if (ValidationHelper.ValidateString(accountInfo.Email, @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$"))
            {
                exception = new Exception("Email address is invalid");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Email_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountInfo.Email);
            }

            if (await accountRepo.IsExist("Email", accountInfo.Email))
            {
                exception = new Exception("Email has been used to create account");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Email_Taken");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountInfo.Email);
            }

            if (!ValidationHelper.ValidateString(accountInfo.Password, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"))
            {
                exception = new Exception("Pasword requires minimum eight characters, at least one letter and one number");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Password_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountInfo.Password);
            }
            
            if (exception != null)
            {
                throw exception;
            }

            var entity = mapper.Map<AccountDTO, Account>(accountInfo);

            foreach (string role in accountInfo.Roles)
            {
                var roleEntity = (await roleRepo.GetPaginated(1, 1, x => x.Name == role, (string) null!)).FirstOrDefault();

                if (roleEntity == null)
                {
                    exception = new Exception("Email has been used to create account");

                    // Add Data to Exception
                    exception.Data.Add("error", "Account_Exception");
                    exception.Data.Add("detail", "Account_Role_Not_Existed");
                    exception.Data.Add("type", "Invalid");
                    exception.Data.Add("value", role);

                    throw exception;
                }

                entity.Roles.Add(roleEntity);
            }
            
            entity.Password = HashPassword(entity.Password); //  Create pasword hash using SHA256 hashing alogrithm

            await this.accountRepo.Create(entity);
        }

        public async Task<AccountDTO> GetAccountInformation(Guid accountId)
        {
            var entity = await accountRepo.GetById(accountId);

            if (entity == null)
            {
                var exception = new Exception("Account information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Existed");
                exception.Data.Add("type", "NotFound");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            return mapper.Map<Account, AccountDTO>(entity);
        }

        public async Task<AccountDTO> GetAccountInformation(string username, string password)
        {
            Expression<Func<Account, bool>> filter = x => (x.Username == username || x.Email == username)&& x.Password == HashPassword(password);
            Expression<Func<Account, object>> include = x => x.Roles;
            Expression<Func<Account, object>> orderBy = x => x.Username;

            var entity = (await accountRepo.GetPaginated(1, 1, include, filter, orderBy, false))
                .FirstOrDefault();

            if (entity == null)
            {
                var exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Incorrect_Credential");
                exception.Data.Add("type", "Unauthorized");
                exception.Data.Add("value", null);

                throw exception;
            }

            return mapper.Map<Account, AccountDTO>(entity);
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountPaginated(int page, int page_size, string username = "", string fullname = "", string email = "", string phone = "", string sortby = "", bool IncludeDeleted = false, bool OnlyVerified = false, bool isDecending = false)
        {
            Expression<Func<Account, bool>> filterExpression = x =>
                    x.Username == username
                    //&& x.Fullname.ToLower().Contains(fullname.ToLower())
                    //&& x.Email.ToLower().Contains(x.Email.ToLower())
                    //&& x.Phone == phone
                    && IncludeDeleted ? true : x.IsDeleted == false
                    && OnlyVerified ? x.IsVerified == true : true;

            Expression<Func<Account, object>> includeExpression = x => x.Roles;

            Expression<Func<Account, object>> sortByExpression = null!;

            switch (sortby)
            {
                case "fullname":
                    sortByExpression = x => x.Fullname;
                    break;
                case "username":
                    sortByExpression = x => x.Username;
                    break;
                default:
                    break;
            }    

            var result = await accountRepo.GetPaginated(page, page_size, includeExpression, filterExpression, sortByExpression, isDecending);

            return mapper.Map<IEnumerable<Account>, IEnumerable<AccountDTO>>(result);
        }

        public async Task RemoveAccount(Guid accountId)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                var exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("statusCode", 400);
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            target.IsDeleted = true;

            await accountRepo.Update(target);
        }

        // TODO: Fix updating account information
        public async Task UpdateAccount(AccountDTO account)
        {
            var target = await accountRepo.GetById(account.Id);

            
            Exception exception = null!;

            // Data validation for account update

            if (target == null)
            {
                exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", account.Id);

                throw exception;
            }

            if (!ValidationHelper.ValidateString(account.Password, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"))
            {
                exception = new Exception("Pasword requires minimum eight characters, at least one letter and one number");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Password_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", account.Password);
            }

            if (exception != null)
            {
                throw exception;
            }

            target.Fullname = account.Fullname;
            target.Password = HashPassword(account.Password);

            await accountRepo.Update(target);
        }

        public async Task UpdateUsername(Guid accountId, string username)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                var exception = new Exception("Account information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            if (await accountRepo.IsExistForUpdate(accountId, "Username", username))
            {
                var exception = new Exception("Username is not available");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Update_Username_Failed_NotAvailable");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", username);

                throw exception;
            }

            target.Username = username;

            await accountRepo.Update(target);
        }
        
        public async Task UpdatePassword(Guid accountId, string password)
        {
            var target = await accountRepo.GetById(accountId);
            Exception exception = null!;
            
            if (target == null)
            {
                exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            // Data validation for account property update
            if (!ValidationHelper.ValidateString(password, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"))
            {
                exception = new Exception("Pasword requires minimum eight characters, at least one letter and one number");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Password_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", password);

                throw exception;
            }

            target.Password = HashPassword(password);

            await accountRepo.Update(target);
        }

        public async Task UpdateEmail(Guid accountId, string email)
        {
            var target = await accountRepo.GetById(accountId);
            Exception exception = null!;

            if (target == null)
            {
                exception = new Exception("Account information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            // Data validation for account property update
            if (!ValidationHelper.ValidateString(email, "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$"))
            {
                exception = new Exception("Email address is invalid");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Email_Invalid");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", email);

                throw exception;
            }

            target.Email = email;

            await accountRepo.Update(target);
        }

        public async Task UpdateFullname(Guid accountId, string fullname)
        {
            var target = await accountRepo.GetById(accountId);
            Exception exception = null!;

            if (target == null)
            {
                exception = new Exception("Account information not found");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            target.Fullname = fullname;

            await accountRepo.Update(target);
        }

        public async Task UpdateVerificationStatus(Guid accountId, bool verifyStatus)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                var exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("error", "Account_Exception");
                exception.Data.Add("detail", "Account_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", accountId);

                throw exception;
            }

            target.IsVerified = verifyStatus;
            await accountRepo.Update(target);
        }

        private byte[] GenerateSalt(int length) 
        {
            return RandomNumberGenerator.GetBytes(length);
        }

        private string HashPassword(string password, byte[] salt)
        {
            byte[] HashedPassword = Rfc2898DeriveBytes.Pbkdf2(password, salt, 3000, HashAlgorithmName.SHA256, 256);
            
            return Convert.ToHexString(HashedPassword);
        }

        private string HashPassword(string password)
        {
            //var salt = GenerateSalt(password.Length);
            byte[] x = Encoding.UTF8.GetBytes(password);
            return HashPassword(password, x);
        }
        public async Task<RoleDTO> GetRole(int roleId)
        {
            var result = await roleRepo.GetById(roleId);

            if (result == null)
            {
                var exception = new Exception("Username or password is incorrect");

                // Add Data to Exception
                exception.Data.Add("error", "Role_Exception");
                exception.Data.Add("detail", "Role_Not_Exsited");
                exception.Data.Add("type", "Invalid");
                exception.Data.Add("value", roleId);

                throw exception;
            }

            return mapper.Map<Role,RoleDTO>(result);
        }

        public async Task<IEnumerable<RoleDTO>> GetRoles()
        {
            return mapper.Map<IEnumerable<Role>, IEnumerable<RoleDTO>>(await roleRepo.GetAll());
        }

        public async Task AddRole(RoleDTO roleInformation)
        {
            var entity = mapper.Map<RoleDTO, Role>(roleInformation);

            await roleRepo.Create(entity);
        }

        public Task UpdateRole(RoleDTO roleInformation)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRole(int roleId)
        {
            throw new NotImplementedException();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
