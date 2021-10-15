using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Exetentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // this is a http response dont have api responses likeBadRequest

    public class MessageHub : Hub
    {
        private readonly IMessageReposetory _messageReposetory;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<Pressencehub> _pressenceHub;
        private readonly PressenceTracker _tracker;
        public MessageHub(IMessageReposetory messageReposetory,
            IUserRepository userRepository, IMapper mapper,
        IHubContext<Pressencehub> pressenceHub, PressenceTracker tracker)
        {
            _tracker = tracker;
            _pressenceHub = pressenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageReposetory = messageReposetory;

        }

        public override async Task OnConnectedAsync()
        {
            //عاوز افهم اكتر الحته ده 
            var httpContext = Context.GetHttpContext();
            // to know which user we click on...
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = getGroupName(Context.User.GetUserName(), otherUser);
           var group =   await AddToGroup(groupName);
           await Clients.Group(groupName).SendAsync("UpdatedGroup",group);



            var messageParams = new MessageParams
            {
                Username = Context.User.GetUserName(),
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageReposetory
                .GetMessagesThread(Context.User.GetUserName(), otherUser, messageParams);

            await Clients.Caller.SendAsync("RecieveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            
           var group= await RemoveFromMessageGroup();
           await Clients.Group(group.name).SendAsync("UpdatedGroup",group);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task sendMessage(CreateMessageDto createMessageDto)
        {
            var user = Context.User.GetUserName();

            if (user == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You can not send messages to yourself");

            var recpient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recpient == null) throw new HubException("Not found user");
            var sender = await _userRepository.GetUserByUsernameAsync(user);

            var message = new Message
            {
                Sender = sender,
                Recipient = recpient,
                RecipientUsername = recpient.UserName,
                SenderUsername = sender.UserName,
                Content = createMessageDto.content
            };
            var groupName = getGroupName(sender.UserName, recpient.UserName);

            var group = await _messageReposetory.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.Username == recpient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recpient.UserName);
                if(connections!= null){
                    await _pressenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new {username = sender.UserName,knownAs = sender.KnownAs});
                }
            }

            _messageReposetory.AddMassege(message);

            if (await _messageReposetory.SaveAllAsynce())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }

        }
        //hubCallerContext gives us username and the connection ID
        private async Task<Group> AddToGroup( string groupName)
        {
            var group = await _messageReposetory.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group == null)
            {
                group = new Group(groupName);
                _messageReposetory.AddGroup(group);
            }

            group.Connections.Add(connection);

            if( await _messageReposetory.SaveAllAsynce()) return group;

            throw new HubException("Failed to join group");

        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageReposetory.GetGroupForConnection(Context.ConnectionId);
           var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
           
            _messageReposetory.RemoveConnection(connection);
           if( await _messageReposetory.SaveAllAsynce()) return group;

            throw new HubException("Failed to remove group");

        }

        private string getGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }



    }
}