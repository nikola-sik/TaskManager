using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITaskManagerRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthenticationController(ITaskManagerRepository repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }

        //POST api/login
        [HttpPost("login")]
        public async Task<ActionResult<UserReadDTO>> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                UserReadDTO authenticatedUser = await AuthenticateUser(userLoginDTO);
                if (authenticatedUser != null)
                {
                    var tokenString = GenerateJSONWebToken(authenticatedUser);
                    return Ok(new { token = tokenString });
                }

                return Unauthorized();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //Get api/logout
        [HttpGet("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                string token = HttpContext.Request.Headers["authorization"].Single().Split(" ")[1];
                if (! (await _repository.ValidateToken(token))) { return Unauthorized(); }

                InvalidToken invalidToken = new InvalidToken();
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();

                DateTime expirationDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                expirationDate = expirationDate.AddSeconds(Double.Parse(claim[3].Value)).ToLocalTime();

                invalidToken.userId = Int32.Parse(claim[0].Value);
                invalidToken.expirationDate = expirationDate;
                invalidToken.token = token;

                await _repository.CreateInvalidToken(invalidToken);
                await _repository.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }

        private async Task<UserReadDTO> AuthenticateUser(UserLoginDTO userLoginDTO)
        {
            UserReadDTO userReadDTO = null;
            var userFromDB = await _repository.GetUserByEmail(userLoginDTO.email);
            if (userFromDB != null && BCrypt.Net.BCrypt.Verify(userLoginDTO.password, userFromDB.password))
            {
                userReadDTO = _mapper.Map<UserReadDTO>(userFromDB);
            }

            return userReadDTO;
        }

        private string GenerateJSONWebToken(UserReadDTO userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddMinutes(60).ToString())

            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;

        }



    }
}