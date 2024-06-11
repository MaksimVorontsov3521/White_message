using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace client_v2
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
                    var response = await client.PostAsync("https://localhost:7777/api/messages/send-group-message", content);

                    // Проверка статуса ответа
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show($"Request error: {e.Message}");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Unexpected error: {e.Message}");
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
                    MessageBox.Show($"Request error: {e.Message}");
                    return null;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Unexpected error: {e.Message}");
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
            public async Task<string> GetnickByLog_in(string login, string password)
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
                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    return "Request error";
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                    return "Unexpected error";
                }
            }
        }
    }
}