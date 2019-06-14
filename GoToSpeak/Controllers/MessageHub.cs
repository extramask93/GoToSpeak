using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GoToSpeak.Data;
using GoToSpeak.Data.GoToSpeak.Data;
using GoToSpeak.Dtos;
using GoToSpeak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoToSpeak.Controllers
{
    [Authorize]
    public class MessageHub : Hub
    {

        private readonly static ConnectionMapping<int> _connections = 
            new ConnectionMapping<int>();
        private readonly DataContext context;
        private readonly IChatRepository chatRepository;
        private readonly IMapper mapper;

        public MessageHub(DataContext context,IChatRepository chatRepository, IMapper mapper)
        {
            this.context = context;
            this.chatRepository = chatRepository;
            this.mapper = mapper;
        }
        public override async Task OnConnectedAsync() {
            var userIdString = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userId = int.Parse(userIdString);
            _connections.Add(userId, Context.ConnectionId);
            var user = await chatRepository.GetUser(userId);
            await RefreshUsers();
            await base.OnConnectedAsync();
        } 
        public override async Task OnDisconnectedAsync(System.Exception exception) {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _connections.Remove(userId, Context.ConnectionId);
            var user = await chatRepository.GetUser(userId);
            await RefreshUsers();
            await base.OnDisconnectedAsync(exception);
        }
        private async Task RefreshUsers() {
            var activeIds = _connections.GetKeys();
            List<User> users = new List<User>();
            foreach(var id in activeIds) {
                var user = await chatRepository.GetUser(id);
                users.Add(user);
            }
            await Clients.All.SendAsync("ActiveUsers", mapper.Map<IEnumerable<UserForListDto>>(users));
        }
        public async Task GetActiveUsers() {
            var activeIds = _connections.GetKeys();
            List<User> users = new List<User>();
            foreach(var id in activeIds) {
                var user = await chatRepository.GetUser(id);
                users.Add(user);
            }
            await Clients.Client(Context.ConnectionId).SendAsync("ActiveUsers", mapper.Map<IEnumerable<UserForListDto>>(users));
        }
        public async void GetHistory(int recipientId)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var messageFromRepo = await chatRepository.GetMessageThread(userId, recipientId);
            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            await Clients.Client(Context.ConnectionId).SendAsync("MessageHistory", messageThread);
        
        }
        public async void SendGlobalMessage(MessageForCreationDto messageForCreationDto) {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var sender = context.Users.FirstOrDefault(u => u.Id == userId);
            var messageToReturn = mapper.Map<MessageToReturnDto>(messageForCreationDto);
            messageToReturn.SenderUsername = sender.UserName;
            messageToReturn.SenderPhotoUrl = sender.PhotoUrl;
            messageToReturn.SenderId = userId;
            await Clients.All.SendAsync("NewGlobalMessage",messageToReturn);
        }
        public async void SendMessage(MessageForCreationDto messageForCreationDto)
        {
            

            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var connections = _connections.GetConnections(messageForCreationDto.RecipientId);
            var connectionsSender = _connections.GetConnections(userId);
            var sender = context.Users.FirstOrDefault(u => u.Id == userId);

            messageForCreationDto.SenderId = userId;
            var recipient = context.Users.FirstOrDefault(u => u.Id == messageForCreationDto.RecipientId);

            if(recipient == null)
                throw new HubException("Could not find user");
            var message = mapper.Map<Message>(messageForCreationDto);
            context.Messages.Add(message);
            if(context.SaveChanges()>0) {
                var messageToReturn = mapper.Map<MessageToReturnDto>(message);
                foreach(var connection in connections) {
                    await Clients.Client(connection).SendAsync("NewMessage",messageToReturn);
                }
                if(userId != messageForCreationDto.RecipientId) {
                    foreach(var connection in connectionsSender) {
                         await Clients.Client(connection).SendAsync("NewMessage",messageToReturn);
                    }
                }
                return;
            }
            throw new HubException("Creating message failed");
        }
    }
}