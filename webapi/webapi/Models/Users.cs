using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Users
    {
        [Key]public int Id { get; set; }
        public string nickname { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public ICollection<Messages> SentMessages { get; set; } = new List<Messages>();
        public ICollection<Messages> ReceivedMessages { get; set; } = new List<Messages>();

    }
}
