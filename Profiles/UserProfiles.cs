using AutoMapper;
using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            //Source -> Target
            CreateMap<User, UserReadDTO>();

            CreateMap<UserCreateDTO, User>();
            
            CreateMap<UserLoginDTO, UserReadDTO>();
        
        }
    }
}