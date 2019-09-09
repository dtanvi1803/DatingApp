using System;

namespace DatingApp.API.DTOs
{
    public class MessageForCreatingDTO
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }
        public MessageForCreatingDTO()
        {
            MessageSent = DateTime.Now;
        }
    }
}