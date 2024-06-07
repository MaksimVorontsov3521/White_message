using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace server
{
    internal class api
    {
        public class MessengerClient
        {
            public class User
            {
                public int Id { get; set; }
                public string nickname { get; set; }
                public string login { get; set; }
                public string password { get; set; }
            }
            public class Message
            {
                public int? MessageId { get; set; }
                public int? SenderId { get; set; }
                public int? ReceiverId { get; set; }
                public string Content { get; set; }
                public DateTime? Timestamp { get; set; }
                public bool IsGroupMessage { get; set; }
                public string? Sendernick { get; set; } // Для удобства
                public string? Receivernick { get; set; }
                public User? Sender { get; set; }
                public User? Receiver { get; set; }

            }

            public class PrivateMessageRequest
            {
                public string Sendernick { get; set; }
                public string Receivernick { get; set; }
                public string Content { get; set; }
            }

            public class LoginRequest
            {
                public string login { get; set; }
                public string Password { get; set; }
            }

            private static readonly HttpClient client = new HttpClient();
            public MessengerClient()
            {

                HttpClient client = new HttpClient(); // Создаем новый экземпляр HttpClient
                client.BaseAddress = new Uri("https://localhost:7777/"); // Устанавливаем BaseAddress
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }                 
            public async Task<User> GetUserByNick(string nickname)
            {
                try
                {
                    return await client.GetFromJsonAsync<User>($"https://localhost:7777/api/users/get-user-by-nickname/{nickname}");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                    return null;
                }
            }
        }
    }
}