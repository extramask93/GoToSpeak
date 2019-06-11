using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Data.GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace GoToSpeak.Controllers
{
    [Authorize]
    public class MessageHub : Hub
    {

        private readonly static ConnectionMapping<int> _connections = 
            new ConnectionMapping<int>();
        private readonly IChatRepository chatRepository;
        private readonly IMapper mapper;

        public MessageHub(IChatRepository chatRepository, IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.mapper = mapper;
        }
        public override async Task OnConnectedAsync() {
            var userIdString = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdString);
            _connections.Add(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        } 
        public override async Task OnDisconnectedAsync(System.Exception exception) {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _connections.Remove(userId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async void GetActiveUsers() {
            var activeIds = _connections.GetKeys();
            List<User> users = new List<User>();
            foreach(var id in activeIds) {
                var user = await chatRepository.GetUser(id);
                users.Add(user);
            }
            await Clients.Client(Context.ConnectionId).SendAsync("ActiveUsers", users);
        }
        public async void GetMessageThread(int recipientId)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var messageFromRepo = await chatRepository.GetMessageThread(userId, recipientId);
            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            await Clients.Client(Context.ConnectionId).SendAsync("MessageThread", messageThread);
        
        }
        public async void SendMessage(MessageForCreationDto messageForCreationDto)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var connections = _connections.GetConnections(userId);
            var sender = await chatRepository.GetUser(userId);
            messageForCreationDto.SenderId = userId;
            var recipient = await chatRepository.GetUser(messageForCreationDto.RecipientId);
            if(recipient == null)
                throw new HubException("Could not find user");
            var message = mapper.Map<Message>(messageForCreationDto);
            chatRepository.Add(message);
            if(await chatRepository.SaveAll()) {
                var messageToReturn = mapper.Map<MessageToReturnDto>(message);
                foreach(var connection in connections) {
                    await Clients.Client(connection).SendAsync("NewMessage",messageToReturn);
                }
                return;
            }
            throw new HubException("Creating message failed");
        }
    }
}