using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConnectumAPI.Models;
using ConnectumAPI.Models.DTOs;
using ConnectumAPI.Persistence.Models.DTOs;
using ConnectumAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConnectumAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var objectList = _userRepository.GetUsers();

            var dto = new List<UserSearchDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<UserSearchDTO>(obj));
            }
            return Ok(dto);
        }
        [HttpGet("{userId:int}")]
        public IActionResult GetUser( int userId)
        {
            var obj = _userRepository.GetUser(userId);
            if (obj == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<UserSearchDTO>(obj);
            return Ok(dto);
        }
        [HttpGet("{userName:alpha}")]
        public IActionResult GetUserByName(string userName)
        {
            var obj = _userRepository.GetUserByUserName(userName);
            if (obj == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<UserSearchDTO>(obj);
            return Ok(dto);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateUser([FromBody] RegisterUserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_userRepository.UserExists(userDTO.UserName))
            {
                ModelState.AddModelError("","User already exists...");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userObj = _mapper.Map<User>(userDTO);
            if (!_userRepository.CreateUser(userObj))
            {
                ModelState.AddModelError("", $"Error when saving record... {userObj.UserName}");
                return StatusCode(500, ModelState);
            }
            return Ok();

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] LoginUserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _userRepository.Authenticate(userDTO.UserName, userDTO.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is not correct..."});
            }
            return Ok(_mapper.Map<UserAuthenticatedDTO>(user));

        }


    }
}