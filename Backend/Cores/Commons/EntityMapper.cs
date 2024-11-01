using AutoMapper;
using Backend.Cores.DTO;
using Backend.Cores.Entities;
using Backend.Cores.ViewModels;

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
                .ForMember(destination => destination.Role, action => action.MapFrom(source => source.Role.Name))
                .ForMember(destination => destination.IsVerified, action => action.MapFrom(source => source.IsVerified))
                .ForMember(destination => destination.IsDeleted, action => action.MapFrom(source => source.IsDeleted))
                .ForMember(destination => destination.CreatedTime, action => action.MapFrom(source => source.CreatedTime))
                .ForMember(destination => destination.Campus, action => action.MapFrom(source => source.CampusId))
                .ForMember(destination => destination.Club, action => action.MapFrom(source => source.ClubId))
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
               .ForMember(destination => destination.CampusId, action => action.MapFrom(source => source.Campus))
               .ForMember(destination => destination.ClubId, action => action.MapFrom(source => source.Club))
               .ForMember(destination => destination.CreatedTime, action => action.MapFrom(source => source.CreatedTime))
               .ForMember(destination => destination.RoleId, action => action.Ignore())
               .ForMember(destination => destination.Role, action => action.Ignore());
    
            CreateMap<AccountDTO, AccountCreationModel>()
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Password, action => action.MapFrom(source => source.Password))
                .ReverseMap();

            CreateMap<AccountDTO, AccountUpdateModel>()
                .ForMember(destination => destination.AccountId, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Password, action => action.MapFrom(source => source.Password))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
                .ForMember(destination => destination.Role, action => action.MapFrom(source => source.Role))
                .ReverseMap();

            CreateMap<AccountDTO, AccountPublicViewModel>()
                .ForMember(destination => destination.Username, action => action.MapFrom(source => source.Username))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Fullname, action => action.MapFrom(source => source.Fullname))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Roles, action => action.MapFrom(source => source.Role))
                .ForMember(destination => destination.CreatedDate, action => action.MapFrom(source => source.CreatedTime))
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

            CreateMap<Token, TokenDTO>()
                .ForMember(destination => destination.Id, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Reason, action => action.MapFrom(source => source.Reason))
                .ForMember(destination => destination.TokenValue, action => action.MapFrom(source => source.Value))
                .ForMember(destination => destination.CreationTime, action => action.MapFrom(source => source.CreatedTime))
                .ForMember(destination => destination.ExpirationTime, action => action.MapFrom(source => source.ExpirationDate))
                .ForMember(destination => destination.ForAccount, action => action.MapFrom(source => source.ValidAccount))
                .ReverseMap();

            // Campus related

            CreateMap<Campus, CampusDTO>().ReverseMap();

            CreateMap<CampusDTO, CampusPublicViewModel>()
                .ForMember(destination => destination.Name, action => action.MapFrom(source => source.Name))
                .ForMember(destination => destination.Email, action => action.MapFrom(source => source.Email))
                .ForMember(destination => destination.Address, action => action.MapFrom(source => source.Address))
                .ForMember(destination => destination.Phone, action => action.MapFrom(source => source.Phone))
                .ForMember(destination => destination.Description, action => action.MapFrom(source => source.Description));


            CreateMap<CampusDTO, CampusCreationModel>()
                .ReverseMap();

            CreateMap<CampusDTO, CampusUpdateModel>()
                .ReverseMap();
            // Club related

            CreateMap<Club, ClubDTO>().ReverseMap();

            CreateMap<ClubDTO, ClubPublicViewModel>();

            // Event related
            CreateMap<Event, EventDTO>()
                .ForMember(destination => destination.Id, action => action.MapFrom(source => source.Id))
                .ForMember(destination => destination.Name, action => action.MapFrom(source => source.Name))
                .ForMember(destination => destination.Description, action => action.MapFrom(source => source.Description))
                .ForMember(destination => destination.Capacity, action => action.MapFrom(source => source.Capacity))
                .ForMember(destination => destination.Price, action => action.MapFrom(source => source.Price))
                .ForMember(destination => destination.StartDate, action => action.MapFrom(source => source.StartDate))
                .ForMember(destination => destination.EndDate, action => action.MapFrom(source => source.EndDate))
                .ForMember(destination => destination.StartTime, action => action.MapFrom(source => source.StartTime))
                .ForMember(destination => destination.EndTime, action => action.MapFrom(source => source.EndTime))
                .ForMember(destination => destination.Club, action => action.MapFrom(source => source.ClubId))
                .ForMember(destination => destination.Image, action => action.MapFrom(source => source.PosterURL))
                .ReverseMap();
        }
    }
}
