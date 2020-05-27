using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public ActionResult<IEnumerable<TaskReadDTO>> GettUserTasks()
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token)) { return Unauthorized(); }

                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var taskItems = _repository.GetUserTasks(loggedUserId);
                return Ok(_mapper.Map<IEnumerable<TaskReadDTO>>(taskItems));
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //GET api/tasks/{id}
        [HttpGet("{id}", Name = "GetTaskById")]
        [Authorize]
        public ActionResult<TaskReadDTO> GetTaskById(int id)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token)) { return Unauthorized(); }

                var taskItem = _repository.GetTaskById(id);
                if (taskItem != null)
                {
                    int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    if (taskItem.userId == loggedUserId)
                        return Ok(_mapper.Map<TaskReadDTO>(taskItem));
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
        public ActionResult<TaskReadDTO> CreateTask(TaskCreateDTO taskCreateDTO)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token)) { return Unauthorized(); }

                string loggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                taskCreateDTO.userId = Int32.Parse(loggedUserId);
                
                if (!_repository.IsAdvancedUser(taskCreateDTO.userId)) taskCreateDTO.text = null;

                var taskModel = _mapper.Map<Task>(taskCreateDTO);
                _repository.CreateTask(taskModel);
                _repository.SaveChanges();

                var taskReadDTO = _mapper.Map<TaskReadDTO>(taskModel);

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
        public ActionResult UpdateTask(int id, TaskUpdateDTO taskUpdateDTO)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token)) { return Unauthorized(); }

                var taskModelFromRepository = _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }

                if (!_repository.IsAdvancedUser(loggedUserId)) taskUpdateDTO.text = null;

                _mapper.Map(taskUpdateDTO, taskModelFromRepository);

                _repository.UpdateTask(taskModelFromRepository);
                _repository.SaveChanges();

                return NoContent();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //PATCH api/tasks/{id}
        [HttpPatch("{id}")]
        [Authorize]
        public ActionResult PartialTaskUpdate(int id, JsonPatchDocument<TaskUpdateDTO> patchDocument)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token))
                {
                    return Unauthorized();
                }
                var taskModelFromRepository = _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }
                var taskToPatch = _mapper.Map<TaskUpdateDTO>(taskModelFromRepository);
                patchDocument.ApplyTo(taskToPatch, ModelState);

                if (!TryValidateModel(taskToPatch))
                {
                    return ValidationProblem(ModelState);
                }

                if (!_repository.IsAdvancedUser(loggedUserId)) taskToPatch.text = null;
                _mapper.Map(taskToPatch, taskModelFromRepository);

                _repository.UpdateTask(taskModelFromRepository);
                _repository.SaveChanges();

                return NoContent();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //DELETE api/tasks/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (!_repository.ValidateToken(token)) { return Unauthorized(); }

                var taskModelFromRepository = _repository.GetTaskById(id);
                if (taskModelFromRepository == null)
                {
                    return NotFound();
                }
                int loggedUserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (taskModelFromRepository.userId != loggedUserId)
                {
                    return Unauthorized();
                }
                _repository.DeleteTask(taskModelFromRepository);
                _repository.SaveChanges();

                return NoContent();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}