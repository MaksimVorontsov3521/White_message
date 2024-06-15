using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                public int? SenderId { get; set; }
                public int? ReceiverId { get; set; }
                public string Content { get; set; }
                public DateTime? Timestamp { get; set; }
                public bool IsGroupMessage { get; set; }
                public string? Sendernick { get; set; } // Для удобства
                public string? Receivernick { get; set; } // Никнейм получателя
                public string? FileName { get; set; }
                public string? FilePath { get; set; }
                public string? FileType { get; set; }
            }

            public class PrivateMessageRequest
            {
                public int SenderId { get; set; }
                public int ReceiverId { get; set; }
                public string Content { get; set; }
            }
            public class GetPrivateMessageRequest
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
            public class UserOnline
            {
                public string UserNick { get; set; }
                public bool IsOnline { get; set; }
            }
            public class ContactsOnline
            {
                public string UserNick { get; set; }
                public bool IsOnline { get; set; }
            }

            private static readonly HttpClient client = new HttpClient();
            public MessengerClient()
            {

                HttpClient client = new HttpClient(); // Создаем новый экземпляр HttpClient
                client.BaseAddress = new Uri("https://localhost:7777/"); // Устанавливаем BaseAddress
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            public async Task<int> SendGroupMessage(string messagecontent, int senderid)
            {
                var message = new Message
                {
                    Content = messagecontent,
                    SenderId = senderid
                };
                try
                {
                    var jsonMessage = JsonSerializer.Serialize(message);
                    var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7777/api/message/send-group-message", content);

                    // Проверка статуса ответа
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return int.Parse(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
                }
                return 0;
            }
            public async Task<List<Message>> GetGroupMessages()
            {
                try
                {
                    var response = await client.GetAsync("https://localhost:7777/api/message/get-group-messages");
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
            public async Task SendPrivateMessage(int senderid, int receiverid, string content)
            {
                try
                {
                    var messageRequest = new PrivateMessageRequest
                    {
                        SenderId = senderid,
                        ReceiverId = receiverid,
                        Content = content
                    };

                    var jsonMessage = JsonSerializer.Serialize(messageRequest);
                    var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://localhost:7777/api/message/send-private-message", httpContent);

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

            public async Task<List<GetPrivateMessageRequest>> GetPrivateMessages(int myId, int receiverId)
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/message/get-private-messages/{myId}/{receiverId}");

                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var messages = JsonSerializer.Deserialize<List<GetPrivateMessageRequest>>(responseBody, new JsonSerializerOptions
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
                    var response = await client.PostAsync("https://localhost:7777/api/user/log-in", httpContent);

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
            public async Task<bool> AddContact(int myId, int contactId)
            {
                var response = await client.PostAsync($"https://localhost:7777/api/user/add-contact/{myId}-{contactId}", null);

                return response.IsSuccessStatusCode;
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
            public async Task<int> GetUserIdByLogin(string login)
            {
                var response = await client.GetAsync($"https://localhost:7777/api/user/get-userid-by-login/{login}");

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
            public async Task<String> GetUsernickById(int id)
            {

                HttpResponseMessage response = await client.GetAsync($"https://localhost:7777/api/user/get-usernick-by-id/{id}");
                return await response.Content.ReadAsStringAsync();
            }
            public async Task<List<UserOnline>> GetAllUsersOnline()
            {
                var response = await client.GetAsync("https://localhost:7777/api/user/get-online-users");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to retrieve users.");
                }

                var users = await response.Content.ReadFromJsonAsync<List<UserOnline>>();
                return users;
            }
            public async Task<bool> GetUserStatus(int userId)
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/user/get-user-status/{userId}");
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error getting user online status: {ex.Message}");
                    return false;
                }
            }
            public async Task<List<ContactsOnline>> GetContactsStatus(int userId)
            {
                try
                {
                    // Отправляем GET-запрос на сервер для получения списка контактов с их статусом онлайн
                    HttpResponseMessage response = await client.GetAsync($"https://localhost:7777/api/user/get-contacts-with-status-by-id/{userId}");

                    // Проверяем, успешно ли выполнен запрос
                    response.EnsureSuccessStatusCode();

                    // Читаем ответ сервера и десериализуем его в список контактов с их статусом онлайн
                    List<ContactsOnline> contactsOnline = await response.Content.ReadFromJsonAsync<List<ContactsOnline>>();

                    return contactsOnline;
                }
                catch (HttpRequestException ex)
                {
                    // Обрабатываем ошибку, если запрос не удалось выполнить
                    Console.WriteLine($"Error getting contacts: {ex.Message}");
                    return null;
                }
            }
            public async Task<String[]> getcontacts(int myId)
            {
                var response = await client.GetAsync($"https://localhost:7777/api/user/get-contacts-by-id/{myId}");
                response.EnsureSuccessStatusCode();
                var contacts = await response.Content.ReadFromJsonAsync<string[]>();
                return contacts;
            }
            public async Task<bool> cheacaccounttologin(string login, string password)
            {
                var request = new LoginRequest
                {
                    login = login,
                    Password = password
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7777/api/user/log-in", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<bool>(responseContent);
                }

                return false;
            }
            public async Task UploadFileAsync(string filePath, int messageId)
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        content.Add(fileContent, "file", Path.GetFileName(filePath));
                        content.Add(new StringContent(messageId.ToString()), "messageId");

                        var response = await client.PostAsync("https://localhost:7777/api/files/upload", content);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            public async Task<Message> GetMessageById(int messageId)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/message/{messageId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Необязательно, если имена свойств в JSON и классе совпадают
                        };

                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            return await JsonSerializer.DeserializeAsync<Message>(responseStream, options);
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception($"Message with ID {messageId} not found.");
                    }
                    else
                    {
                        throw new Exception($"Failed to retrieve message. Status code: {response.StatusCode}");
                    }
                }
            }
            public async Task<byte[]> DownloadFileFromDatabaseAsync(int messageId)
            {
                // Предполагаем, что у вас есть метод для получения файла из базы данных
                var message = await GetMessageById(messageId);
                if (message == null || string.IsNullOrEmpty(message.FilePath))
                {
                    throw new FileNotFoundException("File not found in the database.");
                }

                // Чтение содержимого файла в байтовый массив
                return await File.ReadAllBytesAsync(message.FilePath);
            }

            public async Task DownloadFileAsync(int messageId)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/files/download/{messageId}");
                    response.EnsureSuccessStatusCode();

                    var fileName = response.Content.Headers.ContentDisposition.FileNameStar ?? "downloadedFile";
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(Path.Combine("downloads", fileName), fileBytes);
                }
            }

        }
    }
}