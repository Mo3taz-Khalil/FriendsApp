using API.DTOs;
using API.Entities;
using API.Exetentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiContrroler
    {
        private readonly IUserRepository _UserRepository;
        private readonly IMessageReposetory _MessageReposetory;
        private readonly IMapper _Mapper;

        public MessagesController(IUserRepository userRepository,
            IMessageReposetory messageReposetory, IMapper mapper)
        {
            _UserRepository = userRepository;
            _MessageReposetory = messageReposetory;
            _Mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var user = User.GetUserName();
            if (user == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You can not send messages to yourself");

            var recpient = await _UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recpient == null) return NotFound();
            var sender = await _UserRepository.GetUserByUsernameAsync(user);

            var message = new Message
            {
                Sender = sender,
                Recipient = recpient,
                RecipientUsername = recpient.UserName,
                SenderUsername = sender.UserName,
                Content = createMessageDto.content
            };
            _MessageReposetory.AddMassege(message);

            if (await _MessageReposetory.SaveAllAsynce()) return Ok(_Mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send the message");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _MessageReposetory.GetMessagesForUser(messageParams);

            Response.AddPaginationHeaders(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);


            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username, [FromQuery] MessageParams messageParams)
        {
            var CurrentUsername = User.GetUserName();

            var messages = await _MessageReposetory.GetMessagesThread(CurrentUsername, username, messageParams);

            Response.AddPaginationHeaders(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);


            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deletMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _MessageReposetory.GetMessage(id);

            if (message.Sender.UserName != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            if(message.SenderDeleted&&message.RecipientDeleted) 
            _MessageReposetory.DeleteMessage(message);

            if( await _MessageReposetory.SaveAllAsynce()) return Ok();

            return BadRequest("problem deleting the message");

        }

    }
}
