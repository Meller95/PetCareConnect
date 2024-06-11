using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetCareConnect.Data;
using PetCareConnect.Models;
using System.Collections.Generic;

namespace PetCareConnect.Pages
{
    public class MessagesModel : PageModel
    {
        private readonly MessageRepository _messageRepository;

        public List<Message> Messages { get; set; }
        public List<User> MessagedUsers { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SelectedUserId { get; set; }

        public MessagesModel()
        {
            _messageRepository = new MessageRepository();
        }

        public void OnGet()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;

            MessagedUsers = _messageRepository.GetMessagedUsers(currentUserId);

            if (SelectedUserId.HasValue)
            {
                Messages = _messageRepository.GetMessagesBetweenUsers(currentUserId, SelectedUserId.Value);
            }
            else
            {
                Messages = new List<Message>();
            }
        }
    }
}
