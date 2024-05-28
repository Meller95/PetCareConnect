using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetCareConnect.Data;

namespace PetCareConnect.Pages
{
    public class SendMessageModel : PageModel
    {
        private readonly MessageRepository _messageRepository;

        [BindProperty]
        public string ReceiverUsername { get; set; }
        [BindProperty]
        public string Content { get; set; }

        public SendMessageModel()
        {
            _messageRepository = new MessageRepository();
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var senderUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(senderUsername))
            {
                return RedirectToPage("/SignIn");
            }

            if (ModelState.IsValid)
            {
                _messageRepository.SendMessage(senderUsername, ReceiverUsername, Content);
                return RedirectToPage("/Messages");
            }

            return Page();
        }
    }
}
