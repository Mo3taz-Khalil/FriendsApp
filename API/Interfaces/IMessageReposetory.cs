using API.DTOs;
using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IMessageReposetory
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string GroupName);

        Task<Group> GetGroupForConnection(string connectionId);
        void AddMassege(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int Id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<PagedList<MessageDto>> GetMessagesThread(string CurrentUsername,string RecipientUsername,MessageParams messageParams);

        Task<bool> SaveAllAsynce();
    }
}
