using AutoMapper;
using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Profiles
{
    public class UserTasksProfile : Profile
    {
        public UserTasksProfile()
        {
            //Source -> Target
            CreateMap<UserTask, UserTaskReadDTO>();

            CreateMap<UserTaskCreateDTO, UserTask>();

            CreateMap<UserTaskUpdateDTO, UserTask>();
            
            CreateMap<UserTask, UserTaskUpdateDTO>();
        }
    }
}