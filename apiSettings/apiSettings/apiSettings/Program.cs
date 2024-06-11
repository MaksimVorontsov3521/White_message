using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static server.api;

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
                client.BaseAddress = new Uri("https://localhost:6666/"); // Устанавливаем BaseAddress
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            public async Task AddUser(string nickname, string login, string password)
            {
                var newUser = new User
                {
                    nickname = nickname,
                    login = login,
                    password = password
                };
                var response = await client.PostAsJsonAsync("https://localhost:7777/api/users/add-user", newUser);
                response.EnsureSuccessStatusCode();
            }
            public async Task Delete_user_by_nickname(string nickname)
            {
                var response = await client.DeleteAsync($"https://localhost:7777/api/users/delete-user-by-nickname/{nickname}");
                response.EnsureSuccessStatusCode();
            }
            public async Task ResetAutoIncrementUsers()
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7777/api/Users/reset-autoincrement-users", null);
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
                    var response = await client.PostAsync("https://localhost:7777/api/Messages/reset-autoincrement-messages", null);
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
            public async Task SendGroupMessage(string messagecontent, string nickname)
            {
                var message = new Message
                {
                    Content = messagecontent,
                    Sendernick = nickname
                };
                try
                {
                    var jsonMessage = JsonSerializer.Serialize(message);
                    var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:6666/api/messages/send-group-message", content);

                    // Проверка статуса ответа
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                }
            }
            public async Task<List<Message>> GetGroupMessages()
            {
                try
                {
                    var response = await client.GetAsync("https://localhost:7777/api/messages/get-group-messages");
                    // Проверка статуса ответа
                    response.EnsureSuccessStatusCode();
                    // Чтение и десериализация ответа
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var messages = JsonSerializer.Deserialize<List<Message>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return messages;
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
            public async Task SendPrivateMessage(string senderUsername, string receiverUsername, string content)
            {
                try
                {
                    var messageRequest = new PrivateMessageRequest
                    {
                        Sendernick = senderUsername,
                        Receivernick = receiverUsername,
                        Content = content
                    };

                    var jsonMessage = JsonSerializer.Serialize(messageRequest);
                    var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7777/api/messages/send-private-message", httpContent);

                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: " + responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                }
            }
            public async Task<List<PrivateMessageRequest>> GetPrivateMessages(string nick1, string nick2)
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/messages/get-private-messages/{nick1}/{nick2}");

                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var messages = JsonSerializer.Deserialize<List<PrivateMessageRequest>>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return messages;
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
            public async Task<string> CheckLoginToAcc(string login, string password)
            {
                try
                {
                    var loginRequest = new LoginRequest
                    {
                        login = login,
                        Password = password
                    };
                    var jsonRequest = JsonSerializer.Serialize(loginRequest);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7777/api/users/log-in", httpContent);

                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var trimmedResponse = responseBody.Trim('"');
                    return trimmedResponse;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return "Request error";
                }
            }
            public async Task DeleteAllUsersData()
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync("https://localhost:7777/api/users/delete-all-data-users");
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
                    HttpResponseMessage response = await client.DeleteAsync("https://localhost:7777/api/messages/delete-all-data-messages");
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                }
            }
            public async void ResetAll(MessengerClient messengerClient)
            {
                await messengerClient.DeleteAllMessagesData();
                await messengerClient.DeleteAllUsersData();
                await messengerClient.ResetAutoIncrementMessages();
                await messengerClient.ResetAutoIncrementUsers();
            }
            public async Task<String> getnickbylogin(string login)
            {

                HttpResponseMessage response = await client.GetAsync($"https://localhost:7777/api/users/get-nick-by-login/{login}");
                return await response.Content.ReadAsStringAsync();
            }
            public async void getusers(MessengerClient messengerClient)
            {
                await messengerClient.AddUser("admin", "adminlog", "adminpass");
                await messengerClient.AddUser("worker1", "worker1log", "worker1pass");
                await messengerClient.AddUser("worker2", "worker2log", "worker2pass");
                await messengerClient.AddUser("worker3", "worker3log", "worker3pass");
                await messengerClient.AddUser("бухгалтер", "buglog", "bugpass");
                await messengerClient.AddUser("user1", "user1log", "user1pass");
                await messengerClient.AddUser("user2", "user2log", "user2pass");
            }
        }
        public class Program
        {
            public static async Task Main(string[] args)
            {
                var messengerClient = new MessengerClient();
                string nick = await messengerClient.getnickbylogin("user1log");
                Console.WriteLine(nick);
                //await messengerClient.DeleteAllMessagesData();
                //await messengerClient.ResetAutoIncrementMessagesAsync();
                //await messengerClient.SendGroupMessage("helll","qoq");
                //var messages = await messengerClient.GetGroupMessages();
                //foreach (var message in messages)
                //{
                //    Console.WriteLine($"sendernick: {message.Sendernick}, content: {message.Content}");
                //}
                //var message = await messengerClient.GetPrivateMessages("qoq", "qoq3");
                //foreach (var mess in message)
                //{
                //    Console.WriteLine($"{mess.Sendernick} написал: {mess.Content} для {mess.Receivernick}");
                //}
            }
        }
    }
}