using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoToSpeak.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController: ControllerBase
    {
        private readonly IChatRepository _repo;
        private readonly IMapper _mapper;
        public RoomController(IChatRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }
        [HttpGet("history/{roomName}")]
        public async Task<IActionResult> GetMessageHistory(string roomName, [FromQuery]MessageParams param)
        {
            var list  = await _repo.GetRoomHistory(roomName, param);
            Response.AddPagination(list.CurrentPage, list.PageSize, list.TotalCount, list.TotalPages);
            var listToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(list);
            return Ok(listToReturn);
        }
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms([FromQuery]RoomParams param)
        {
            var rooms = await _repo.GetRooms(param);
            Response.AddPagination(rooms.CurrentPage, rooms.PageSize, rooms.TotalCount, rooms.TotalPages);
            var roomToReturn = _mapper.Map<IEnumerable<RoomToReturn>>(rooms);
            return Ok(roomToReturn);
        }
    }
}