using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Helpers;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GoToSpeak.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IChatRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHubContext<MessageHub> _hub;

        public MessagesController(IChatRepository repo, IMapper mapper,
                                IHubContext<MessageHub> hub)
        {
            _repo = repo;
            _mapper = mapper;
            _hub = hub;
        }
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);
        }
 
        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId,[FromQuery]MessageParams messageParams)
        {
            
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();     
            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId,messageParams);          
            Response.AddPagination(messageFromRepo.CurrentPage, messageFromRepo.PageSize, messageFromRepo.TotalCount, messageFromRepo.TotalPages);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);
            
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody]MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId);
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);
            if(recipient == null)
                return BadRequest("Could not find user");
            var message = _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(message);
            if(await _repo.SaveAll()) {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id},messageToReturn);
            }
            throw new Exception("Creating message failed");
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId) {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo.SenderId == userId) {
                messageFromRepo.SenderDeleted = true;
            }
            if(messageFromRepo.RecipientId == userId) {
                messageFromRepo.RecipientDeleted = true;
            }
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted) {
                _repo.Delete(messageFromRepo);
            }
            if(await _repo.SaveAll())
                return NoContent();
            throw new Exception("Error deleting the message");
        }
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id) {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var message = await _repo.GetMessage(id);
            if(message.RecipientId != userId)
                return Unauthorized();
            message.IsRead = true;
            message.DateRead = DateTime.Now;
            await _repo.SaveAll();
            return NoContent();
        }

    }
}