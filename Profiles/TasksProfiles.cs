using AutoMapper;
using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Profiles
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            //Source -> Target
            CreateMap<Task, TaskReadDTO>();

            CreateMap<TaskCreateDTO, Task>();

            CreateMap<TaskUpdateDTO, Task>();
            
            CreateMap<Task, TaskUpdateDTO>();
        }
    }
}