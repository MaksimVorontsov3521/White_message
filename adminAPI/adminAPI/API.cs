using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace adminAPI
{
    public class API
    {
        public class MessengerClient
        {
            public class User
            {
                public int Id { get; set; }
                public string fio { get; set; }
                public string login { get; set; }
                public string password { get; set; }
                public string post { get; set; }
                public bool isOnline { get; set; }
                public string? contacts { get; set; }
                public string? usernick { get; set; }
            }
            public class Message
            {
                public int? MessageId { get; set; }
                public int SenderId { get; set; }
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

            private static readonly HttpClient client = new HttpClient();
            public MessengerClient()
            {

                HttpClient client = new HttpClient(); // Создаем новый экземпляр HttpClient
                client.BaseAddress = new Uri("https://localhost:6666/"); // Устанавливаем BaseAddress
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            public async Task AddUser(string login, string password, string fio, string post)
            {
                var newUser = new User
                {
                    fio = fio,
                    usernick = $"{fio} ({post})",
                    post = post,
                    login = login,
                    password = password,
                    isOnline = false
                };
                var response = await client.PostAsJsonAsync("https://localhost:7777/api/user/add-user", newUser);
                response.EnsureSuccessStatusCode();
            }
            public async Task UpdateUser (User user)
            {
                var response = await client.PostAsJsonAsync("https://localhost:7777/api/user/update-user", user);
                response.EnsureSuccessStatusCode();
            }
            public async Task UpdateMessage(Message message)
            {
                var response = await client.PostAsJsonAsync("https://localhost:7777/api/message/update-message", message);
                response.EnsureSuccessStatusCode();
            }
            public async Task Delete_user_by_id(int id)
            {
                var response = await client.DeleteAsync($"https://localhost:7777/api/user/delete-user-by-id/{id}");
                response.EnsureSuccessStatusCode();
            }
            public async Task ResetAutoIncrementUsers()
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7777/api/user/reset-autoincrement-user", null);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("Автоинкремент users сброшен");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            public async Task ResetAutoIncrementMessages()
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7777/api/Messages/reset-autoincrement-message", null);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("Автоинкремент messages сброшен");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Ошибка: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
          
            public async Task DeleteAllUsersData()
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync("https://localhost:7777/api/user/delete-all-data-user");
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                }
            }
            public async Task DeleteAllMessagesData()
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync("https://localhost:7777/api/message/delete-all-data-message");
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                }
            }
          
            public async Task<bool> AddContact(int myId, int contactId)
            {
                var response = await client.PostAsync($"https://localhost:7777/api/user/add-contact/{myId}-{contactId}", null);

                return response.IsSuccessStatusCode;
            }
            public async Task<bool> DeleteContacts(int myId)
            {
                var response = await client.DeleteAsync($"https://localhost:7777/api/user/delete-contacts-by-id/{myId}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            public async Task<List<User>> GetAllUsers()
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/user");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var users = JsonSerializer.Deserialize<List<User>>(jsonString, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return users;
                    }
                    else
                    {
                        throw new Exception("Failed to load users.");
                    }
                }
            }
            public async Task<List<Message>> GetAllMessages()
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/message");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var messages = JsonSerializer.Deserialize<List<Message>>(jsonString, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return messages;
                    }
                    else
                    {
                        throw new Exception("Failed to load messages.");
                    }
                }
            }
        }
        }
    }
