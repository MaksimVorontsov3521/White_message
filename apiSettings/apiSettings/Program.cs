using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static server.api;
using static server.api.MessengerClient;

namespace server
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
            public class PrivateMessageRequest
            {
                public int SenderId { get; set; }
                public int ReceiverId { get; set; }
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
            public async Task SendGroupMessage(string messagecontent, int senderid)
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
                    Console.WriteLine($"Request error: {e.Message}");
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e.Message}");
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

            public async Task DeleteAllUsers()
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync("https://localhost:7777/api/user/delete-all-users");
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
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
            public async void ResetAll(MessengerClient messengerClient)
            {
                await messengerClient.DeleteAllMessagesData();
                await messengerClient.DeleteAllUsersData();
                await messengerClient.ResetAutoIncrementMessages();
                await messengerClient.ResetAutoIncrementUsers();
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
            public async Task<String> GetUsernickById(int id)
            {

                HttpResponseMessage response = await client.GetAsync($"https://localhost:7777/api/user/get-usernick-by-id/{id}");
                return await response.Content.ReadAsStringAsync();
            }
            //public async void getusers(MessengerClient messengerClient)
            //{
            //    await messengerClient.AddUser("admin", "adminlog", "adminpass");
            //    await messengerClient.AddUser("worker1", "worker1log", "worker1pass");
            //    await messengerClient.AddUser("worker2", "worker2log", "worker2pass");
            //    await messengerClient.AddUser("worker3", "worker3log", "worker3pass");
            //    await messengerClient.AddUser("бухгалтер", "buglog", "bugpass");
            //    await messengerClient.AddUser("user1", "user1log", "user1pass");
            //    await messengerClient.AddUser("user2", "user2log", "user2pass");
            //}


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


            public async Task<String[]> getcontacts(int myId)
            {
                var response = await client.GetAsync($"https://localhost:7777/api/user/get-contacts-by-id/{myId}");
                response.EnsureSuccessStatusCode();
                var contacts = await response.Content.ReadFromJsonAsync<string[]>();
                return contacts;
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
                    return null; // Или выбросить исключение, если пользователь не найден
                }
                else
                {
                    throw new Exception($"Error fetching user by ID: {response.ReasonPhrase}");
                }
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
            public async Task<List<ContactsOnline>> GetContacts(int userId)
            {
                try
                {
                    // Отправляем GET-запрос на сервер для получения списка контактов с их статусом онлайн
                    HttpResponseMessage response = await client.GetAsync($"https://localhost:7777/api/user/get-contacts-by-id/{userId}");

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
            public async Task<List<User>> GetAllUsers()
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://localhost:7777/api/user/get-all-users");
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
                    var response = await client.GetAsync($"https://localhost:7777/api/message/get");
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



        public class Program
        {
            public static async Task Main(string[] args)
            {
                int userid;
                var messengerClient = new MessengerClient();
                var messages = await messengerClient.GetAllMessages();
                foreach (var message in messages)
                {
                    Console.WriteLine($"messageid - {message.MessageId} senderid - {message.SenderId} content - {message.Content} usernick - {message.Sendernick}");
                }
                //await messengerClient.SetUserOffline(5);
                //bool q = await messengerClient.GetUserStatus(5);
                //Console.WriteLine(q);
                //await messengerClient.SetUserOffline(5);
                //await messengerClient.SetUserOffline(7);
                //await messengerClient.SetUserOffline(6);
                //var users = await messengerClient.GetAllUsersOnline();
                //foreach (var user in users)
                //{
                //    Console.WriteLine($"usernick: {user.UserNick}, isonline: {user.IsOnline}");
                //}
                //await messengerClient.DeleteContacts(5);
                //await messengerClient.AddContact(5,5);
                //await messengerClient.AddContact(5, 7);
                //await messengerClient.AddContact(5, 10);
                //var contacts = await messengerClient.getcontacts(5);
                //foreach (var contact in contacts)
                //{
                //    Console.WriteLine(contact);
                //}

                //await messengerClient.DeleteAllUsers();

                //var user = await messengerClient.GetUserById(5);
                //Console.WriteLine($"id: {user.Id}, fio: {user.fio}, usernick: {user.usernick}, login: {user.login}, password: {user.password}, post: {user.post}");
                //string usernick = await messengerClient.GetUsernickById(5);
                //Console.WriteLine(usernick);
                //await messengerClient.DeleteAllMessagesData();
                //await messengerClient.DeleteAllUsersData();
                //await messengerClient.AddUser("user4log", "user4pass", "Григорьев Виталий Палыч", "Рабочий");
                //await messengerClient.AddUser("user4log", "user4pass", "Витальич Андрей Петрович", "Директор");
                //userid = await messengerClient.GetUserIdByNickAsync("Какой то Кто то Дмитриевич (Уборщик)");
                //userid = await messengerClient.GetUserIdByNickAsync("Кто то тот (Уборщик?)");
                //Console.WriteLine(userid);
                //await messengerClient.Delete_user_by_id(8);
                //await messengerClient.SendGroupMessage("qoqs", userid);
                //await messengerClient.DeleteContacts("admin");
                //await messengerClient.AddContact("admin", "user2");
                //var contacts = await messengerClient.getcontacts("admin");
                //foreach (var contact in contacts)
                //{
                //    Console.WriteLine(contact);
                //}                
                //await messengerClient.DeleteContacts("admin");
                //string nick = await messengerClient.getnickbylogin("user1log");
                //Console.WriteLine(nick);
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