using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class User
    {
        [Key]public int Id { get; set; }
        public string fio {  get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string post { get; set; }
        public bool isOnline { get; set; }
        public string? usernick { get; set; }
        public string? contacts { get; set; }
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
            

    }
}
