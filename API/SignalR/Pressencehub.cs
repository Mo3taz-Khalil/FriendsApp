using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exetentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class Pressencehub : Hub
    {
        private readonly PressenceTracker _tracker;
        public Pressencehub(PressenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
            }

            var currentUser = _tracker.getOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUser);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
           var isOffline= await _tracker.userDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());


            await base.OnDisconnectedAsync(exception);
        }


    }
}