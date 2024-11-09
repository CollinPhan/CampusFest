using AutoMapper;
using Backend.API.Services.Interface;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.Exceptions;
using Backend.Cores.ViewModels;
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
        private readonly IBaseRepository<Account> accountRepo;
        private readonly IBaseRepository<Role> roleRepo;
        private readonly IMapper mapper;
        private bool disposedValue;

        public AccountService(IBaseRepository<Account> accountRepo, IBaseRepository<Role> roleRepository, IMapper mapper)
        {
            this.accountRepo = accountRepo;
            this.roleRepo = roleRepository;
            this.mapper = mapper;
        }

        public async Task AddAccount(AccountDTO accountInfo)
        {
            var entity = mapper.Map<Account>(accountInfo);

            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdatedTime = DateTime.UtcNow;

            // Adding role
            Role roleEntity = (await roleRepo.FindFirstMatch(x => x.Name == accountInfo.Role))!;

            entity.RoleId = roleEntity.Id;
            entity.Role = roleEntity;

            // Create a hashed password.
            entity.Password = HashPassword(entity.Password); //  Create pasword hash using SHA256 hashing alogrithm

            // Add new Account entity into the database.
            await this.accountRepo.Create(entity);
        }

        public async Task<AccountDTO> GetAccountInformation(Guid accountId)
        {
            var entity = await accountRepo.GetById(accountId);

            if (entity == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    "Account information not found",
                    "Account_Exception",
                    "notFound",
                    "Account information not found",
                    "Account information not found for given Id",
                    accountId);

                return null!;
            }

            return mapper.Map<Account, AccountDTO>(entity);
        }

        public async Task<AccountDTO> GetAccountInformation(string username, string password)
        {
            Expression<Func<Account, bool>> predicate = x => (x.Username == username || x.Email == username) && x.Password == HashPassword(password);

            var entity = await accountRepo.FindFirstMatch(predicate, x => x.Role);

            if (entity == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Username or password is invalid",
                   error: "Account_Login_Exception",
                   type: "Invalid",
                   summary: "Wrong username or password",
                   detail: "Wrong username or password",
                   value: username);
                return null!;
            }

            return mapper.Map<Account, AccountDTO>(entity);
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountPaginated(int page, int page_size, List<string> includes, Expression<Func<Account, bool>> predicate)
        {
            var result = await accountRepo.GetPaginated(page, page_size, includes, predicate, null!);

            return mapper.Map<IEnumerable<AccountDTO>>(result);
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountPaginated(int page, int page_size, string username = "", string fullname = "", string email = "", string phone = "", string sortby = "", bool IncludeDeleted = false, bool OnlyVerified = false, bool isDecending = false)
        {
            Expression<Func<Account, bool>> filterExpression = x =>
                    x.Username.Contains(username)
                    //&& x.Fullname.ToLower().Contains(fullname.ToLower())
                    //&& x.Email.ToLower().Contains(x.Email.ToLower())
                    //&& x.Phone == phone
                    && (IncludeDeleted ? true : x.IsDeleted == false)
                    && (OnlyVerified ? x.IsVerified == true : true);

            IEnumerable<string> includeProperty = new List<string> { "Role" };

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

            var result = await accountRepo.GetPaginated(page, page_size, includeProperty, filterExpression, sortByExpression, false, isDecending);

            return mapper.Map<IEnumerable<Account>, IEnumerable<AccountDTO>>(result);
        }

        public async Task RemoveAccount(Guid accountId)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Deletion_Exception",
                   type: "Invalid",
                   summary: "User id is not valid",
                   detail: @"There is no account with provided id exist",
                   value: accountId);
                return;
            }

            target.IsDeleted = true;

            await accountRepo.Update(target);
        }

        public async Task UpdateAccount(AccountDTO account)
        {
            var target = await accountRepo.GetById(account.Id);

            // Data validation for account update
            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: account.Id);
                return;
            }

            if (target.IsDeleted)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account state is invalid",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Can not update a deleted account",
                   detail: @"This account is marked as deleted. No futher update operations can be used on this account.",
                   value: account.Id);
                return;
            }

            if (!ValidationHelper.ValidateString(account.Password, @"^(?=.*[A-Za-z])(?=.*\d)[\d\w!@#$%^&*_]{8,30}$"))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Password is invalid",
                    error: "Account_Update_Exception",
                    type: "Invalid",
                    summary: "New password is invalid",
                    detail: @"The password length should be between 8 character in length, you might use special characters such as '!@#$%^&*_' while creating password.",
                    value: account.Password);
                return;
            }

            target.Fullname = account.Fullname;
            target.Email = account.Email;
            target.LastUpdatedTime = DateTime.UtcNow;
            target.Phone = account.Phone;
            target.Password = HashPassword(account.Password);

            await accountRepo.Update(target);
        }

        public async Task UpdateUsername(Guid accountId, string username)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: accountId);
                return;
            }

            if (await accountRepo.IsExistForUpdate(accountId, "Username", username))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Username is already taken",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Username is not available",
                   detail: @"Already exists an account with the same username.",
                   value: accountId);
                return;
            }

            target.Username = username;

            await accountRepo.Update(target);
        }

        public async Task UpdatePassword(Guid accountId, string password)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: accountId);
                return;
            }

            // Data validation for account property update
            if (!ValidationHelper.ValidateString(target.Password, @"^(?=.*[A-Za-z])(?=.*\d)[\d\w!@#$%^&*_]{8,30}$"))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Password is invalid",
                    error: "Account_Update_Exception",
                    type: "Invalid",
                    summary: "New password is invalid",
                    detail: @"The password length should be between 8 character in length, you might use special characters such as '!@#$%^&*_' while creating password.",
                    value: password);
            }

            target.Password = HashPassword(password);

            await accountRepo.Update(target);
        }

        public async Task UpdateEmail(Guid accountId, string email)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: accountId);
                return;
            }

            // Data validation for account property update
            if (accountRepo.FindFirstMatch(x => x.Id != accountId && x.Email.ToLower() == email.ToLower() && !x.IsDeleted) != null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Email hase been taken by another account",
                    error: "Account_Update_Exception",
                    type: "Invalid",
                    summary: "The email address is not available to create account.",
                    detail: @"The given email address has been used to create an existing account in the system.",
                    value: email);
                return;
            }

            // Data validation for account property update
            if (!ValidationHelper.ValidateString(email, @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$"))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                  message: "Email is not valid",
                  error: "Account_Update_Exception",
                  type: "Invalid",
                  summary: "The email is invalid.",
                  detail: @"Please check for any type in your email.",
                  value: email);
                return;
            }

            target.Email = email;

            await accountRepo.Update(target);
        }

        public async Task UpdateFullname(Guid accountId, string fullname)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: accountId);
                return;
            }

            target.Fullname = fullname;

            await accountRepo.Update(target);
        }

        public async Task UpdateVerificationStatus(Guid accountId, bool verifyStatus)
        {
            var target = await accountRepo.GetById(accountId);

            if (target == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Account does not exist",
                   error: "Account_Update_Exception",
                   type: "Invalid",
                   summary: "Account does not exist.",
                   detail: @"There is no account exist for the provided accountId.",
                   value: accountId);
                return;
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
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Role does not exist",
                   error: "Role_Exception",
                   type: "Invalid",
                   summary: "Role does not exist.",
                   detail: @"There is no role exist for the provided roleId.",
                   value: roleId);
                return null!;
            }

            return mapper.Map<Role, RoleDTO>(result);
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

        public async Task UpdateRole(RoleDTO roleInformation)
        {
            var entity = await roleRepo.GetById(roleInformation.Id);

            if (entity == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Role does not exist",
                   error: "Role_Exception",
                   type: "Invalid",
                   summary: "Role does not exist.",
                   detail: @"There is no role exist for the provided roleId.",
                   value: roleInformation.Id);
                return;
            }

            entity.Name = roleInformation.Name;

            await roleRepo.Update(entity);
        }

        public Task RemoveRole(int roleId)
        {
            ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "You can not delete role!",
                   error: "Role_Deletion_Exception",
                   type: "Invalid",
                   summary: "You can not remove system role",
                   detail: @"This function will cause inconsistency and data loss so we decided to just barred everyone from using it. :p",
                   value: roleId);
            return null!;
        }

        public async Task<bool> ValidateAccountRegisterInformation(AccountDTO account)
        {
            // Check for existing username inside the database.
            if (await accountRepo.IsExist("Username", account.Username))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Username has been taken",
                    error: "Account_Creation_Exception",
                    type: "Invalid",
                    summary: "Username has been taken",
                    detail: "There is another account has been created with this username, please try another one.",
                    value: account.Username);
            }

            // Checking for any existing active account with the same email address.
            if (await accountRepo.FindFirstMatch(x => x.Email == account.Email && !x.IsDeleted) != null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Email address is invalid",
                    error: "Account_Creation_Exception",
                    type: "Invalid",
                    summary: "Email address is invalid",
                    detail: "The email address has already been taken.",
                    value: account.Email);
            }

            // Checking for a valid email address.
            if (!ValidationHelper.ValidateEmail(account.Email))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Email address is invalid",
                    error: "Account_Creation_Exception",
                    type: "Invalid",
                    summary: "Email address is invalid",
                    detail: "The email address does not valid. Please retry with another email (Ex: exapmle@gmail.com)",
                    value: account.Phone);
            }

            if (!ValidationHelper.ValidatePhoneNumber(account.Phone))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                   message: "Phone number is invalid",
                   error: "Account_Creation_Exception",
                   type: "Invalid",
                   summary: "Phone number is invalid",
                   detail: "The phone number should be 9 to 11 digit in length.",
                   value: account.Phone);
            }

            // Checking for a valid password.
            if (!ValidationHelper.ValidatePassword(account.Password))
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Password is invalid",
                    error: "Account_Creation_Exception",
                    type: "Invalid",
                    summary: "Password is invalid",
                    detail: @"The password should at least 8 character in length with at least one numeric digit, you also can use special characters such as '!@#$%^&*_' while creating password.",
                    value: account.Password);
            }
            // Checking for valid role.
            var roleEntity = (await roleRepo.FindFirstMatch(x => x.Name == account.Role));

            if (roleEntity == null)
            {
                ExceptionGenerator.GenericServiceException<BaseServiceException>(
                    message: "Password is invalid",
                    error: "Account_Creation_Exception",
                    type: "Invalid",
                    summary: "Role not found",
                    detail: @"Account role does not match with any existing role in the system",
                    value: account.Role);
            }

            return true;
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
