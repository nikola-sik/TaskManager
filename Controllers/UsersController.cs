using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITaskManagerRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(ITaskManagerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //POST api/users
        [HttpPost(Name = "CreateUser")]
        public async Task<ActionResult<UserReadDTO>> CreateUser(UserCreateDTO userCreateDTO)
        {
            try
            {
                var hashedPassword =  BCrypt.Net.BCrypt.HashPassword(userCreateDTO.password, 10);
                userCreateDTO.password = hashedPassword;
                var userModel = _mapper.Map<User>(userCreateDTO);
                try
                {
                   await _repository.CreateUser(userModel);
                }
                catch (Exception e)
                {
                    if ("Email alredy exists!".Equals(e.Message))
                    {
                        return BadRequest(new { error = "Email alredy exists!" });
                    }
                    throw e;
                }
                await _repository.SaveChanges();

                var userReadDTO = _mapper.Map<UserReadDTO>(userModel);
                return CreatedAtRoute(nameof(CreateUser), userReadDTO);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}