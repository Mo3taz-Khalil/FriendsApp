using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PressenceTracker
    {
        private static readonly Dictionary<string, List<string>> onlineUsers =
            new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool  isOnline=false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(username))
                {
                    if (!onlineUsers[username].Contains(connectionId))
                    {
                        onlineUsers[username].Add(connectionId);
                    }
                }
                else
                {
                    onlineUsers.Add(username, new List<string> { connectionId });
                    isOnline=true;
                }
            }
            return Task.FromResult(isOnline);
        }


        public Task<bool> userDisconnected(string username, string connectionId)
        {
            bool isOffline=false;
            lock (onlineUsers)
            {
                if (!onlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                onlineUsers[username].Remove(connectionId);
                if (onlineUsers[username].Count == 0)
                {
                    onlineUsers.Remove(username);
                    isOffline=true;
                }
            }
            return Task.FromResult(isOffline);
        }


        public Task<string[]> getOnlineUsers()
        {
            string[] OnlineUsers;
            lock (onlineUsers)
            {
                OnlineUsers = onlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(OnlineUsers);
        }


        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (onlineUsers)
            {
                connectionIds = onlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }

    }
}