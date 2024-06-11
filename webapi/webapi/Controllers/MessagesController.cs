using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using webapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public class PrivateMessageRequest
        {
            public string Sendernick { get; set; }
            public string Receivernick { get; set; }
            public string Content { get; set; }
        }

        // Метод для получения всех сообщений
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Messages>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // Метод для получения сообщения по его ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Messages>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        [HttpPost("reset-autoincrement-messages")]
        public async Task<IActionResult> ResetAutoIncrement_messages()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Messages', RESEED, 0);");
                return Ok("Автоинкремент messages сброшен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }


        [HttpPost("send-group-message")]
        public async Task<IActionResult> SendGroupMessage([FromBody] Messages message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content) || message.SenderId <= 0)
            {

                return BadRequest("Invalid message data.");
            }

            try
            {
                message.Timestamp = DateTime.UtcNow;
                message.IsGroupMessage = true;
                var sender = await _context.Users.FirstOrDefaultAsync(u => u.nickname == message.Sendernick);
                if (sender == null)
                {
                    return NotFound("Sender not found.");
                }
                message.SenderId = sender.Id;
                message.Sender = sender;
                // Получение объекта отправителя из базы данных
                sender = await _context.Users.FindAsync(message.SenderId);
                if (sender == null)
                {
                    return NotFound("Sender not found.");
                }
                // Установка никнейма отправителя и навигационного свойства
                // Если сообщение не групповое, получаем объект получателя из базы данных
                if (message.ReceiverId.HasValue)
                {
                    var receiver = await _context.Users.FindAsync(message.ReceiverId);
                    if (receiver == null)
                    {
                        return NotFound("Receiver not found.");
                    }

                    // Установка никнейма получателя и навигационного свойства
                    message.Receivernick = receiver.nickname;
                    message.Receiver = receiver;
                }

                _context.Messages.Add(message);               
                await _context.SaveChangesAsync();
                return Ok(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("get-group-messages")]
        public async Task<IActionResult> GetGroupMessages()
        {
            var messages = await _context.Messages
                .Where(m => m.IsGroupMessage)
                .Include(m => m.Sender) // Включаем данные о отправителе
                .OrderBy(m => m.MessageId)
                .ToListAsync();

            var result = messages.Select(m => new
            {
                m.Content,
                Sendernick = m.Sender.nickname  // Добавляем имя отправителя               
            });
            return Ok(result);
        }
     
        [HttpPost("send-private-message")]
        public async Task<IActionResult> SendPrivateMessage([FromBody] PrivateMessageRequest request)
        {
            var sender = await _context.Users.SingleOrDefaultAsync(u => u.nickname == request.Sendernick);
            var receiver = await _context.Users.SingleOrDefaultAsync(u => u.nickname == request.Receivernick);
            if (sender == null || receiver == null)
            {
                Console.WriteLine($"sender: {request.Sendernick}, receiver: {request.Receivernick}");
                return BadRequest("Sender or receiver not found.");
            }
            var message = new Messages
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Content = request.Content,
                Timestamp = DateTime.UtcNow,
                IsGroupMessage = false,
                Sendernick = sender.nickname,
                Receivernick = receiver.nickname
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }


        [HttpGet("get-private-messages/{nick1}/{nick2}")]
        public async Task<IActionResult> GetPrivateMessages(string nick1, string nick2)
        {
            var messages = await _context.Messages
                .Where(m => m.IsGroupMessage == false &&
                            ((m.Sendernick == nick1 && m.Receivernick == nick2) ||
                             (m.Sendernick == nick2 && m.Receivernick == nick1)))
                .Include(m => m.Sender) // Включаем данные о отправителе
                .Include(m => m.Receiver) // Включаем данные о получателе
                .OrderBy(m => m.MessageId)
                .ToListAsync();

            var result = messages.Select(m => new
            {
                m.Content,
                Sendernick = m.Sender.nickname,
                Receivernick = m.Receiver.nickname
            });

            return Ok(result);
        }

        [HttpDelete("delete-all-data-messages")]
        public async Task<IActionResult> DeleteAllData_messages()
        {
            _context.Messages.RemoveRange(await _context.Messages.ToListAsync());
            await _context.SaveChangesAsync();
            Console.WriteLine("mess");
            return NoContent();
        }
    }
}