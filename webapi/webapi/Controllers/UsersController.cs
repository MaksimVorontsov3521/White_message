using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webapi.Data;
using webapi.Models;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public class LoginRequest
        {
            public string login { get; set; }
            public string Password { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }



        [HttpPost("add-user")]
        public async Task<ActionResult<Users>> AddUser(Users user)
        {
            // Добавление пользователя в контекст
            _context.Users.Add(user);

            // Сохранение изменений в базе данных
            await _context.SaveChangesAsync();

            // Возвращение ответа с данными добавленного пользователя
            Console.WriteLine("чел был добавлен");
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpDelete("delete-user-by-nickname/{nickname}")]
        public async Task<ActionResult<Users>> DeleteUser(string nickname)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.nickname == nickname);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
        [HttpGet("get-user-by-nickname/{nickname}")]
        public async Task<ActionResult<Users>> GetUserbynickname(string nickname)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.nickname == nickname);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpGet("get-nick-by-login/{login}")]
        public async Task<IActionResult> Getnickbylogin(string login)
        {
            Console.WriteLine(1);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.login == login);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.nickname);
        }


        [HttpDelete("delete-all-data-users")]
        public async Task<IActionResult> DeleteAllData_messages()
        {
            _context.Users.RemoveRange(await _context.Users.ToListAsync());
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("reset-autoincrement-users")]
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

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.login == request.login && u.password == request.Password);

            if (user == null)
            {
                return Ok(false); // или return Ok("Invalid username or password.");
            }

            return Ok(user.nickname); // или return Ok("Login successful.");
        }
    }
}
