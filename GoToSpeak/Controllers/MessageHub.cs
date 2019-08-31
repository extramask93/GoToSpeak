using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Data.GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GoToSpeak.Controllers
{
    public class MessageHub : Hub<IMessageHub>
    {

        private readonly static ConnectionMapping<int> _connectionsMapping =
            new ConnectionMapping<int>();
        private readonly static List<RoomToReturn> _Rooms = new List<RoomToReturn>();
        public readonly static List<UserForListDto> _Connections = new List<UserForListDto>();
        private readonly DataContext context;
        private readonly IChatRepository chatRepository;
        private readonly IMapper mapper;
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(DataContext context, IChatRepository chatRepository, IMapper mapper, ILogger<MessageHub> logger)
        {
            _logger = logger;
            this.context = context;
            this.chatRepository = chatRepository;
            this.mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            var userIdString = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdString);
            var user = await chatRepository.GetUser(userId);
            var userForListDto = mapper.Map<UserForListDto>(user);
            userForListDto.CurrentRoom = "";
            _Connections.Add(userForListDto);
            _connectionsMapping.Add(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _connectionsMapping.Remove(userId, Context.ConnectionId);
            var user = _Connections.Where(u => u.Id == userId).First();
            if(!String.IsNullOrEmpty(user.CurrentRoom))
                await Clients.OthersInGroup(user.CurrentRoom).RemoveUser(user);
            _Connections.Remove(user);
            await base.OnDisconnectedAsync(exception);
        }
        public void Send(string roomName, string message)
        {
            if (message.StartsWith("/private"))
                SendPrivate(message);
            else
                SendToRoom(roomName, message);
        }
        private int Identity()
        {
            return int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        public void SendPrivate(string message)
        {
            // message format: /private(receiverName) Lorem ipsum...
            string[] split = message.Split(')');
            string receiver = split[0].Split('(')[1];
            // Who is the sender;
            var sender = _Connections.Where(u => u.Id == Identity()).First();
            message = Regex.Replace(message, @"\/private\(.*?\)", string.Empty).Trim();

            var receiverUser = context.Users.FirstOrDefault(user => user.UserName == receiver);
            // Build the message TODO
            MessageToReturnDto messageViewModel = new MessageToReturnDto();
            foreach (var cc in _connectionsMapping.GetConnections(receiverUser.Id))
            {
                Clients.Client(cc).NewMessage(messageViewModel);
            }
            Clients.Caller.NewMessage(messageViewModel);
        }
        public void SendToRoom(string roomName, string message)
        {
            try
            {
                var user = context.Users.Where(u => u.Id == Identity()).FirstOrDefault();
                var room = context.Rooms.Where(r => r.Name == roomName).FirstOrDefault();
                // Create and save message in database
                Message msg = new Message()
                {
                    Content = Regex.Replace(message, @"(?i)<(?!img|a|/a|/img).*?>", String.Empty),
                    MessageSent = DateTime.Now,
                    SenderId = user.Id,
                    ToRoom = room
                };
                context.Messages.Add(msg);
                context.SaveChanges();
                // Broadcast the message
                var messageViewModel = mapper.Map<MessageToReturnDto>(msg);
                Clients.Group(roomName).NewMessage(messageViewModel);
            }
            catch (Exception)
            {
                Clients.Caller.OnError("Message not send!");
            }
        }
        public void CreateRoom(string roomName)
        {
            try
            {
                // Accept: Letters, numbers and one space between words.
                Match match = Regex.Match(roomName, @"^\w+( \w+)*$");
                if (!match.Success)
                {
                    Clients.Caller.OnError("Invalid room name!\nRoom name must contain only letters and numbers.");
                }
                else if (roomName == "lobby")
                {
                    Clients.Caller.OnError("lobby is a reserved name");
                }
                else if (roomName.Length < 5 || roomName.Length > 20)
                {
                    Clients.Caller.OnError("Room name must be between 5-20 characters!");
                }
                else if (context.Rooms.Any(r => r.Name == roomName))
                {
                    Clients.Caller.OnError("Another chat room with this name exists");
                }
                else
                {
                    // Create and save chat room in database
                    var user = context.Users.Where(u => u.Id == Identity()).FirstOrDefault();
                    var room = new Room()
                    {
                        Name = roomName,
                        CreatorId = user.Id
                    };
                    context.Rooms.Add(room);
                    context.SaveChanges();

                    if (room != null)
                    {
                        // Update room list
                        var roomViewModel = mapper.Map<RoomToReturn>(room);
                        _Rooms.Add(roomViewModel);
                        Clients.All.AddChatRoom(roomViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.OnError("Couldn't create chat room: " + ex.Message);
            }
        }
        public void DeleteRoom(string roomName)
        {
            try
            {
                // Delete from database
                var messages = context.Messages.Where(m => m.ToRoom.Name == roomName);
                context.Messages.RemoveRange(messages);
                context.SaveChanges();
                var room = context.Rooms.Where(r => r.Name == roomName).FirstOrDefault();
                context.Rooms.Remove(room);
                context.SaveChanges();

                // Delete from list
                var roomViewModel = _Rooms.First<RoomToReturn>(r => r.Name == roomName);
                _Rooms.Remove(roomViewModel);

                // Move users back to Lobby
                Clients.Group(roomName).OnRoomDeleted(string.Format("Room {0} has been deleted.\nYou are now moved to the Lobby!", roomName));

                // Tell all users to update their room list
                Clients.All.RemoveChatRoom(roomViewModel);
            }
            catch (Exception)
            {
                Clients.Caller.OnError("Can't delete this chat room.");
            }
        }
        public IEnumerable<MessageToReturnDto> GetMessageHistory(string roomName)
        {
            var messageHistory = context.Messages.Include(u => u.Sender).Where(m => m.ToRoom.Name == roomName)
                    .OrderByDescending(m => m.MessageSent)
                    .Take(20)
                    .AsEnumerable()
                    .Reverse()
                    .ToList();

            return mapper.Map<IEnumerable<MessageToReturnDto>>(messageHistory);
        }
        public IEnumerable<UserForListDto> GetUsers(string roomName)
        {
            return _Connections.Where(u => u.CurrentRoom == roomName).ToList();
        }
        public async Task<IEnumerable<RoomToReturn>> GetRooms()
        {
            // First run?
            if (_Rooms.Count == 0)
            {
                var rooms = await chatRepository.GetRooms();
                foreach (var room in rooms)
                {
                    var roomToReturn = mapper.Map<RoomToReturn>(room);
                    _Rooms.Add(roomToReturn);
                }
            }
            return _Rooms.ToList();
        }
        public void Join(string roomName)
        {
            try
            {
                var user = _Connections.Where(u => u.Id == Identity()).FirstOrDefault();
                if (user.CurrentRoom != roomName)
                {
                    // Remove user from others list
                    if (!string.IsNullOrEmpty(user.CurrentRoom))
                    {
                        Clients.OthersInGroup(user.CurrentRoom).RemoveUser(user);
                        Leave(user.CurrentRoom);
                    }

                    // Join to new chat room

                    Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    user.CurrentRoom = roomName;

                    // Tell others to update their list of users
                    Clients.OthersInGroup(roomName).AddUser(user);
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.OnError("You failed to join the chat room!" + roomName + ex.Message);
            }
        }

        private void Leave(string roomName)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public async void SendGlobalMessage(MessageForCreationDto messageForCreationDto)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var sender = context.Users.FirstOrDefault(u => u.Id == userId);
            var messageToReturn = mapper.Map<MessageToReturnDto>(messageForCreationDto);
            messageToReturn.SenderUsername = sender.UserName;
            messageToReturn.SenderPhotoUrl = sender.PhotoUrl;
            messageToReturn.SenderId = userId;
            await Clients.All.NewGlobalMessage(messageToReturn);
        }
        public async void SendMessage(MessageForCreationDto messageForCreationDto)
        {


            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var connections = _connectionsMapping.GetConnections(messageForCreationDto.RecipientId);
            var connectionsSender = _connectionsMapping.GetConnections(userId);
            var sender = context.Users.FirstOrDefault(u => u.Id == userId);

            messageForCreationDto.SenderId = userId;
            var recipient = context.Users.FirstOrDefault(u => u.Id == messageForCreationDto.RecipientId);

            if (recipient == null)
                throw new HubException("Could not find user");
            var message = mapper.Map<Message>(messageForCreationDto);
            context.Messages.Add(message);
            if (context.SaveChanges() > 0)
            {
                var messageToReturn = mapper.Map<MessageToReturnDto>(message);
                foreach (var connection in connections)
                {
                    await Clients.Client(connection).NewMessage(messageToReturn);
                }
                if (userId != messageForCreationDto.RecipientId)
                {
                    foreach (var connection in connectionsSender)
                    {
                        await Clients.Client(connection).NewMessage(messageToReturn);
                    }
                }
                return;
            }
            throw new HubException("Creating message failed");
        }
    }
}