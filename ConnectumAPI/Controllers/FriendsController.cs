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
    public class FriendsController : ControllerBase
    {
        private IFriendRepository _friendRepository;
        private readonly IMapper _mapper;

        public FriendsController(IFriendRepository friendRepository, IMapper mapper)
        {
            _friendRepository = friendRepository;
            _mapper = mapper;
        }

        [HttpGet("friends/{User1ID:int}")]
        public IActionResult GetFriends(int User1ID)
        {
            var objectList = _friendRepository.GetFriends(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("pending/{User1ID:int}")]
        public IActionResult GetPending(int User1ID)
        {
            var objectList = _friendRepository.GetPending(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("block1/{User1ID:int}")]
        public IActionResult GetBlockedUser1(int User1ID)
        {
            var objectList = _friendRepository.GetBlockedUser1(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("block2/{User1ID:int}")]
        public IActionResult GetBlockedUser2(int User1ID)
        {
            var objectList = _friendRepository.GetBlockedUser2(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("blockboth/{User1ID:int}")]
        public IActionResult GetBlockedUserBoth(int User1ID)
        {
            var objectList = _friendRepository.GetBlockedUserBoth(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("requests/{User1ID:int}")]
        public IActionResult GetRequests(int User1ID)
        {
            var objectList = _friendRepository.GetRequests(User1ID);

            var dto = new List<FriendDTO>();

            foreach (var obj in objectList)
            {
                dto.Add(_mapper.Map<FriendDTO>(obj));
            }
            return Ok(dto);
        }

        [HttpGet("relationship/{User1ID:int}/{User2ID:int}")]
        public IActionResult GetRelationship(int User1ID, int User2ID)
        {
            var obj = _friendRepository.GetRelationship(User1ID, User2ID);
            if (obj == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<FriendDTO>(obj);
            return Ok(dto.Type);
        }

        [HttpGet("friend/{User1ID:int}/{User2ID:int}")]
        public IActionResult GetFriend(int User1ID, int User2ID)
        {
            var obj = _friendRepository.GetFriend(User1ID, User2ID);
            if (obj == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<FriendDTO>(obj);
            return Ok(dto.Type);
        }

        [AllowAnonymous]
        [HttpPost("delete")]
        public IActionResult DeleteRelationship([FromBody] FriendDTO friendDTO)
        {
            if (friendDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!_friendRepository.RelationshipExists(friendDTO.User1ID, friendDTO.User2ID))
            {
                ModelState.AddModelError("", "Relationship doesn't exists...");
                return StatusCode(404, ModelState);
            }
            var friendObj = _mapper.Map<Friend>(friendDTO);
            var obj = _friendRepository.DeleteRelationship(friendObj);
            if (obj == false)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult CreateRelationship([FromBody] FriendDTO friendDTO)
        {
            if (friendDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_friendRepository.RelationshipExists(friendDTO.User1ID, friendDTO.User2ID))
            {
                ModelState.AddModelError("", "Relationship already exists...");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var friendObj = _mapper.Map<Friend>(friendDTO);
            if (!_friendRepository.CreateRelationship(friendObj))
            {
                ModelState.AddModelError("", $"Error when saving relationship record... {friendObj.User1ID}/{friendObj.User2ID}");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("update")]
        public IActionResult UpdateRelationship([FromBody] FriendDTO friendDTO)
        {
            if (friendDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!_friendRepository.RelationshipExists(friendDTO.User1ID, friendDTO.User2ID))
            {
                ModelState.AddModelError("", "Relationship doesn't exists...");
                return StatusCode(404, ModelState);
            }
            var friendObj = _mapper.Map<Friend>(friendDTO);
            var obj = _friendRepository.UpdateRelationship(friendObj);
            if (obj == false)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
