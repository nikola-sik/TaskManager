using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskManagerRepository _repository;
        private readonly IMapper _mapper;

        public TasksController(ITaskManagerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //GET api/tasks
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserTaskReadDTO>>> GettUserTasks()
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token)) { return Unauthorized(); }

                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var taskItems = await _repository.GetUserTasks(loggedUserId);
                return Ok(_mapper.Map<IEnumerable<UserTaskReadDTO>>(taskItems));
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //GET api/tasks/{id}
        [HttpGet("{id}", Name = "GetTaskById")]
        [Authorize]
        public async Task<ActionResult<UserTaskReadDTO>> GetTaskById(int id)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token)) { return Unauthorized(); }

                var taskItem = await _repository.GetTaskById(id);
                if (taskItem != null)
                {
                    int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (taskItem.userId == loggedUserId)
                        return Ok(_mapper.Map<UserTaskReadDTO>(taskItem));
                    else
                        return Unauthorized();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //POST api/tasks
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserTaskReadDTO>> CreateTask(UserTaskCreateDTO taskCreateDTO)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token)) { return Unauthorized(); }

                string loggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                taskCreateDTO.userId = Int32.Parse(loggedUserId);
                
                if (! await _repository.IsAdvancedUser(taskCreateDTO.userId)) taskCreateDTO.text = null;

                var taskModel = _mapper.Map<UserTask>(taskCreateDTO);
                await _repository.CreateTask(taskModel);
                await _repository.SaveChanges();

                var taskReadDTO = _mapper.Map<UserTaskReadDTO>(taskModel);

                return CreatedAtRoute(nameof(GetTaskById), new { id = taskReadDTO.id }, taskReadDTO);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //PUT api/task/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateTask(int id, UserTaskUpdateDTO taskUpdateDTO)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token)) { return Unauthorized(); }

                var taskModelFromRepository = await _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }

                if (! await _repository.IsAdvancedUser(loggedUserId)) taskUpdateDTO.text = null;

                _mapper.Map(taskUpdateDTO, taskModelFromRepository);

                await _repository.UpdateTask(taskModelFromRepository);

                return Ok(_mapper.Map<UserTaskReadDTO>(taskModelFromRepository));
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //PATCH api/tasks/{id}
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> PartialUserTaskUpdate(int id, JsonPatchDocument<UserTaskUpdateDTO> patchDocument)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token))
                {
                    return Unauthorized();
                }
                var taskModelFromRepository = await _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }
                var taskToPatch = _mapper.Map<UserTaskUpdateDTO>(taskModelFromRepository);
                patchDocument.ApplyTo(taskToPatch, ModelState);

                if (!TryValidateModel(taskToPatch))
                {
                    return ValidationProblem(ModelState);
                }

                if (! await _repository.IsAdvancedUser(loggedUserId)) taskToPatch.text = null;
                _mapper.Map(taskToPatch, taskModelFromRepository);

                await _repository.UpdateTask(taskModelFromRepository);

                return Ok(_mapper.Map<UserTaskReadDTO>(taskModelFromRepository));

            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //DELETE api/tasks/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! await _repository.ValidateToken(token)) { return Unauthorized(); }

                var taskModelFromRepository = await _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }
                await _repository.DeleteTask(taskModelFromRepository);

                return NoContent();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}