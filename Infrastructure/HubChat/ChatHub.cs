using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Back.Infrastructure.HubChat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;
        public ChatHub(IChatRepository chatRepository) 
        {
            _chatRepository = chatRepository;
        }
        public async Task SendMessage(Guid chatId, Guid senderId, string message)
        {
            var msg = new Message(chatId, senderId, message);
            await _chatRepository.AddMessageAsync(chatId, msg);

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", senderId, message);
        }


        public override async Task OnConnectedAsync()
        {
            // Можно добавить в группу (чат)
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"];
            if (!string.IsNullOrEmpty(chatId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }
            await base.OnConnectedAsync();
        }
    }
}
