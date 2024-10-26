using AutoMapper;
using Backend.Cores.Entities;
using Backend.Cores.ViewModels;
using Backend.Infrastructures.Data.DTO;

namespace Backend.Cores.Commons
{
    public class EntityMapper: Profile
    {
        /// <summary>
        ///  Initiate Mapper Profile between destinations and domain entities.
        /// </summary>
        public EntityMapper() 
        {
            /// Account related 

            // From Account Entity to Account DTO
            CreateMap<Account, AccountDTO>()
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
                .ForMember(destination => destination.Password, action => action.MapFrom(source => source.Password))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Roles, action => action.MapFrom(source => from role in source.Roles select role.Name))
                .ForMember(destination => destination.IsVerified, action => action.MapFrom(source => source.IsVerified))
                .ForMember(destination => destination.IsDeleted, action => action.MapFrom(source => source.IsDeleted))
                .ForMember(destination => destination.CreatedTime, action => action.MapFrom(source => source.CreatedTime))
                .ForMember(destination => destination.LastUpdated, action => action.MapFrom(source => source.LastUpdatedTime));

            // From Account DTO to Account Entity
            CreateMap<AccountDTO, Account>()
               .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
               .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
               .ForMember(destination => destination.Password, action => action.MapFrom(source => source.Password))
               .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
               .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
               .ForMember(destination => destination.IsVerified, action => action.MapFrom(source => source.IsVerified))
               .ForMember(destination => destination.IsDeleted, action => action.MapFrom(source => source.IsDeleted))
               .ForMember(destination => destination.CreatedTime, action => action.MapFrom(source => source.CreatedTime));

            CreateMap<AccountDTO, AccountCreationModel>()
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Password, action => action.MapFrom(source => source.Password))
                .ReverseMap();

            CreateMap<AccountDTO, AccountUpdateModel>()
                .ForMember(destination => destination.AccountId, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
                .ForMember(destination => destination.Roles, action => action.MapFrom(source => source.Roles))
                .ReverseMap();

            CreateMap<AccountDTO, AccountPublicViewModel>()
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Roles, action => action.MapFrom(source => source.Roles))
                .ReverseMap();

            // Role related

            CreateMap<Role, RoleDTO>()
                .ForMember(destination => destination.Id, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Name, action => action.MapFrom(source => source.Name))
                .ForMember(destination => destination.Permission, action => action.MapFrom(source => new List<string>())) // Need to update database
                .ReverseMap();

            CreateMap<RoleDTO, RoleCreationModel>()
                .ForMember(destination => destination.Name, action => action.MapFrom(source => source.Name))
                .ForMember(destination => destination.Description, action => action.MapFrom(source => source.Description))
                .ForMember(destination => destination.Permission, action => action.MapFrom(source => source.Permission))
                .ReverseMap();

            CreateMap<RoleDTO, RoleViewModel>()
                .ForMember(destination => destination.Name, action => action.MapFrom(source => source.Name))
                .ForMember(destination => destination.Description, action => action.MapFrom(source => source.Description))
                .ForMember(destination => destination.Permission, action => action.MapFrom(source => source.Permission))
                .ReverseMap();

            // Token related

            CreateMap <Token, TokenDTO>()
                .ForMember(destination => destination.Id, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Reason, action => action.MapFrom(source => source.Reason))
                .ForMember(destination => destination.TokenValue, action => action.MapFrom(source => source.Value))
                .ForMember(destination => destination.CreationTime, action => action.MapFrom(source => source.CreatedTime))
                .ForMember(destination => destination.ExpirationTime, action => action.MapFrom(source => source.ExpirationDate))
                .ForMember(destination => destination.ForAccount, action => action.MapFrom(source => source.ValidAccount))
                .ReverseMap();
        }
    }
}
