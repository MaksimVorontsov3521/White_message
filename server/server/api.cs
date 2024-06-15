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
                public int id { get; set; }
                public string fio { get; set; }
                public string login { get; set; }
                public string password { get; set; }
                public string post { get; set; }
                public string usernick { get; set; }
            }
            public class Message
            {
                public int? MessageId { get; set; }
                public int? SenderId { get; set; }
                public int? ReceiverId { get; set; }
                public string Content { get; set; }
                public DateTime? Timestamp { get; set; }
                public bool IsGroupMessage { get; set; }
                public string? Sendernick { get; set; }
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

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7777/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }                 
            public async Task<User> GetUserByNick(string nickname)
            {
                try
                {
                    return await client.GetFromJsonAsync<User>($"https://localhost:7777/api/user/get-user-by-nickname/{nickname}");
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
            public async Task<User> GetUserById(int id)
            {
                var response = await client.GetAsync($"https://localhost:7777/api/user/get-user-by-id/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<User>();
                    return user;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Error fetching user by ID: {response.ReasonPhrase}");
                }
            }
            public async Task<int> GetUserIdByNick(string usernick)
            {
                var response = await client.GetAsync($"https://localhost:7777/api/user/get-userid-by-usernick/{usernick}");

                if (response.IsSuccessStatusCode)
                {
                    var userId = await response.Content.ReadFromJsonAsync<int>();
                    return userId;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return 0;
                }
                else
                {
                    throw new Exception("Error fetching user ID");
                }
            }

            public async Task SetUserOnline(int userId)
            {
                var response = await client.PostAsync($"https://localhost:7777/api/user/set-online/{userId}", null);
                response.EnsureSuccessStatusCode();
            }

            public async Task SetUserOffline(int userId)
            {
                var response = await client.PostAsync($"https://localhost:7777/api/user/set-offline/{userId}", null);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}