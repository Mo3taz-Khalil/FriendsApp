using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageReposetory
    {
        private readonly DataContext _Context;
        private readonly IMapper _Mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _Context = context;
            _Mapper = mapper;
        }

        public void AddMassege(Message message)
        {
            _Context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _Context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int Id)
        {
            return await _Context.Messages
                .Include(x=>x.Sender)
                .Include(x=>x.Recipient)
                .SingleOrDefaultAsync(x=>x.Id==Id);

        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _Context.Messages
                 .OrderByDescending(m => m.MessageSent)
                 .AsQueryable();



            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username
                        &&m.RecipientDeleted==false),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username
                        && m.SenderDeleted==false),
                _ => query.Where(m => m.Recipient.UserName == messageParams.Username
                        && m.RecipientDeleted==false && m.DateRead == null)
            };
            var messages = query.ProjectTo<MessageDto>(_Mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreatAsync(messages, messageParams.PageNumber, messageParams.pageSize);



        }

        public async Task<PagedList<MessageDto>> GetMessagesThread(string CurrentUsername,
             string RecipientUsername,MessageParams messageParams)
        {
            //await
            var messages =  _Context.Messages
                .Include(m => m.Sender.Photos)
                .Include(r => r.Recipient.Photos)
                .Where(x => x.Sender.UserName == CurrentUsername && x.Recipient.UserName == RecipientUsername&& x.RecipientDeleted==false
                    || x.Sender.UserName == RecipientUsername && x.Recipient.UserName == CurrentUsername&& x.SenderDeleted==false)
                    .OrderBy(x => x.MessageSent)
                    // .ToListAsync()
                    .AsQueryable();


            var unreadMessages = messages.Where(m => m.DateRead == null &&
                m.Recipient.UserName == CurrentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                await _Context.SaveChangesAsync();
            }

           var query=  messages.ProjectTo<MessageDto>(_Mapper.ConfigurationProvider);  
            
            return await PagedList<MessageDto>.CreatAsync(query, messageParams.PageNumber, messageParams.pageSize);

          //  return _Mapper.Map<IEnumerable<MessageDto>>(messages);    
        }

        public async Task<bool> SaveAllAsynce()
        {
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}
