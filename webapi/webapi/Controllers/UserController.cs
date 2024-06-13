using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webapi.Data;
using webapi.Models;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
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

        public static class ContactHelper
        {
            public static string SerializeContacts(List<string> contacts)
            {
                return string.Join(";", contacts);
            }

            public static List<string> DeserializeContacts(string? contacts)
            {
                return contacts?.Split(';').ToList() ?? new List<string>();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }
        [HttpGet("get-userid-by-usernick/{usernick}")]
        public async Task<int> GetUserIdByNickAsync(string usernick)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.usernick == usernick);
            if (user == null)
            {
                return 0; // или выбросить исключение, если пользователь не найден
            }

            return user.Id;
        }

        [HttpGet("get-userid-by-login/{login}")]
        public async Task<int> GetUserIdByLoginAsync(string login)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.login == login);
            if (user == null)
            {
                return 0; // или выбросить исключение, если пользователь не найден
            }

            return user.Id;
        }
        [HttpGet("get-user-by-id/{id}")]
        public async Task<ActionResult<User>> GetUserbynickname(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpGet("get-usernick-by-id/{id}")]
        public async Task<IActionResult> Getnickbylogin(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.usernick);
        }

        [HttpGet("get-contacts-with-status-by-id/{userId}")]
        public async Task<IActionResult> GetContactswithstatus(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Десериализуем список контактов пользователя и создаем список объектов ContactsOnline
            var contacts = ContactHelper.DeserializeContacts(user.contacts);
            var contactsOnline = contacts.Select(contact => new ContactsOnline
            {
                UserNick = contact,
                IsOnline = CheckIfUserIsOnline(contact) // Предположим, что у вас есть метод для проверки статуса онлайн
            }).ToList();

            return Ok(contactsOnline);
        }
        public bool CheckIfUserIsOnline(string userNick)
        {
            var user = _context.User.FirstOrDefault(u=>u.usernick == userNick);
            return user.isOnline;
        }

        [HttpGet("get-contacts-by-id/{userId}")]
        public async Task<IActionResult> GetContacts(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Десериализуем список контактов пользователя и возвращаем его
            var contacts = ContactHelper.DeserializeContacts(user.contacts);
            return Ok(contacts);
        }
        [HttpGet("get-online-users")]
        public async Task<IActionResult> GetonlineUsers()
        {
            var users = await _context.User
                .Select(u => new UserOnline
                {
                    UserNick = u.usernick,
                    IsOnline = u.isOnline
                })
                .ToListAsync();

            return Ok(users);
        }
        [HttpGet("get-user-status/{id}")]
        public async Task<bool?> GetUserOnlineStatusByIdAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return null; // или можно выбросить исключение, если пользователь не найден
            }

            return user.isOnline;
        }
        //[HttpGet("get-contacts-by-usernick/{usernick}")]
        //public async Task<ActionResult<String[]>> Getcontacts(string usernick)
        //{
        //    int i = 0;
        //    var user = await _context.User.FirstOrDefaultAsync(u => u.usernick == usernick);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    if (string.IsNullOrEmpty(user.contacts))
        //    {
        //        return Ok(Array.Empty<string>());
        //    }

        //    string[] contacts = user.contacts.Split(' ');
        //    return Ok(contacts);
        //}

        [HttpPost("set-online/{id}")]
        public async Task<IActionResult> SetOnline(int id)
        {
            Console.WriteLine(id);
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.isOnline = true;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("set-offline/{id}")]
        public async Task<IActionResult> SetOffline(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.isOnline = false;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("add-user")]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            // Добавление пользователя в контекст
            _context.User.Add(user);
            // Сохранение изменений в базе данных
            await _context.SaveChangesAsync();

            // Возвращение ответа с данными добавленного пользователя
            Console.WriteLine("чел был добавлен");
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpPost("reset-autoincrement-user")]
        public async Task<IActionResult> ResetAutoIncrement_users()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Users', RESEED, 0);");
                return Ok("Автоинкремент users сброшен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
        [HttpPost("log-in")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.login) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid login request.");
            }

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.login == request.login && u.password == request.Password);

            if (user == null)
            {
                return Ok(false); // или return Ok("Invalid username or password.");
            }

            return Ok(true); // или return Ok("Login successful.");
        }

        [HttpPost("add-contact/{myId}-{contactId}")]
        public async Task<IActionResult> AddContact(int myId, int contactId)
        {
            var user = await _context.User.FindAsync(myId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var contact = await _context.User.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound("Contact user not found.");
            }

            var contacts = ContactHelper.DeserializeContacts(user.contacts);
            if (contacts.Contains(contact.usernick))
            {
                return BadRequest("Contact already exists.");
            }
            if (contact.usernick==user.usernick)
            {
                return BadRequest("Вы не можете добавить себя в контакты");
            }

            contacts = ContactHelper.DeserializeContacts(user.contacts);
            contacts.Add(contact.usernick);
            user.contacts = ContactHelper.SerializeContacts(contacts);
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }




        [HttpDelete("delete-user-by-id/{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpDelete("delete-all-data-user")]
        public async Task<IActionResult> DeleteAllData_messages()
        {
            _context.User.RemoveRange(await _context.User.ToListAsync());
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete-contacts-by-id/{myId}")]
        public async Task<IActionResult> Deletecontacts(int myId)
        {
            var user = await _context.User.FindAsync(myId);
            if (user == null)
            {
                return NotFound();
            }

            user.contacts = null;
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

