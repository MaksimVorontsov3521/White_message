using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace webapi.Models
{
    public class Message
    {
        [Key]public int? MessageId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool IsGroupMessage { get; set; }
        public string? Sendernick { get; set; } // Для удобства
        public string? Receivernick { get; set; } // Никнейм получателя

        [JsonIgnore]
        public User? Sender { get; set; }
        [JsonIgnore]
        public User? Receiver { get; set; }
    }
}
