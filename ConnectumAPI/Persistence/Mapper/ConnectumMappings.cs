using AutoMapper;
using ConnectumAPI.Models;
using ConnectumAPI.Models.DTOs;
using ConnectumAPI.Persistence.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Mapper
{
    public class ConnectumMappings : Profile
    {
        public ConnectumMappings()
        {
            CreateMap<User, UserSearchDTO>().ReverseMap();
            CreateMap<User, RegisterUserDTO>().ReverseMap();
            CreateMap<User, LoginUserDTO>().ReverseMap();
            CreateMap<User, UserAuthenticatedDTO>().ReverseMap();
            CreateMap<Friend, FriendDTO>().ReverseMap();
        }
    }
}
