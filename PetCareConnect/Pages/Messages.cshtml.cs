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

        public MessagesModel()
        {
            _messageRepository = new MessageRepository();
        }

        public void OnGet()
        {
            var LoggedInUserId = HttpContext.Session.GetInt32("UserId");
            var currentUserId = HttpContext.Session.GetInt32("UserId").Value;
            Messages = _messageRepository.GetMessages(currentUserId);
        }
    }
}
