using Microsoft.AspNetCore.Mvc;
using webapi.Data;
using webapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Reflection;
using System;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MessageController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public class PrivateMessageRequest
        {
            public int SenderId { get; set; }
            public int ReceiverId { get; set; }
            public string Content { get; set; }
        }
        
        // Метод для получения всех сообщений
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Message.ToListAsync();
        }
        // Метод для получения сообщения по его ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessageById(int id)
        {
            var message = await _context.Message.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        [HttpGet("get-group-messages")]
        public async Task<IActionResult> GetGroupMessages()
        {
            var messages = await _context.Message
                .Where(m => m.IsGroupMessage)
                .Include(m => m.Sender) // Включаем данные о отправителе
                .OrderBy(m => m.MessageId)
                .ToListAsync();

            var result = messages.Select(m => new
            {
                m.Content,
                Sendernick = $"{m.Sender.fio} ({m.Sender.post})"// Добавляем имя отправителя               
            });
            return Ok(result);
        }

        [HttpGet("get-private-messages/{Id1}/{Id2}")]
        public async Task<IActionResult> GetPrivateMessages(int Id1, int Id2)
        {
            var messages = await _context.Message
                .Where(m => m.IsGroupMessage == false &&
                            ((m.SenderId == Id1 && m.ReceiverId == Id2) ||
                             (m.SenderId == Id2 && m.ReceiverId == Id1)))
                .Include(m => m.Sender) // Включаем данные о отправителе
                .Include(m => m.Receiver) // Включаем данные о получателе
                .OrderBy(m => m.MessageId)
                .ToListAsync();

            var result = messages.Select(m => new
            {
                m.Content,
                Sendernick = $"{m.Sender.fio} ({m.Sender.post})",
                Receivernick = $"{m.Receiver.fio} ({m.Receiver.post})",
            });

            return Ok(result);
        }

        [HttpPost("reset-autoincrement-messages")]
        public async Task<IActionResult> ResetAutoIncrement_messages()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Message', RESEED, 0);");
                Console.WriteLine("Автоинкемент message сброшен");
                return Ok("Автоинкремент messages сброшен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }


        [HttpPost("send-group-message")]
        public async Task<IActionResult> SendGroupMessage([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content) || message.SenderId <= 0)
            {

                return BadRequest("Invalid message data.");
            }
            try
            {
                message.Timestamp = DateTime.UtcNow;
                message.IsGroupMessage = true;
                var sender = await _context.User.FirstOrDefaultAsync(u => u.Id == message.SenderId);
                if (sender == null)
                {
                    return NotFound("Sender not found.");
                }
                message.Sender = sender;
                // Получение объекта отправителя из базы данных
                sender = await _context.User.FindAsync(message.SenderId);
                message.Sendernick = sender.usernick;
                if (sender == null)
                {
                    return NotFound("Sender not found.");
                }
                // Установка никнейма отправителя и навигационного свойства
                // Если сообщение не групповое, получаем объект получателя из базы данных              
                _context.Message.Add(message);               
                await _context.SaveChangesAsync();
                return Ok(message.MessageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("send-private-message")]
        public async Task<IActionResult> SendPrivateMessage([FromBody] PrivateMessageRequest request)
        {
            var sender = await _context.User.SingleOrDefaultAsync(u => u.Id == request.SenderId);
            var receiver = await _context.User.SingleOrDefaultAsync(u => u.Id == request.ReceiverId);
            if (sender == null || receiver == null)
            {
                Console.WriteLine($"sender: {request.SenderId}, receiver: {request.ReceiverId}");
                return BadRequest("Sender or receiver not found.");
            }
            var message = new Message
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Content = request.Content,
                Timestamp = DateTime.UtcNow,
                IsGroupMessage = false,
                Sendernick = sender.usernick,
                Receivernick = receiver.usernick
            };
            _context.Message.Add(message);
            await _context.SaveChangesAsync();
            return Ok(message.MessageId);
        }
        [HttpDelete("delete-all-data-message")]
        public async Task<IActionResult> DeleteAllData_messages()
        {
            _context.Message.RemoveRange(await _context.Message.ToListAsync());
            await _context.SaveChangesAsync();
            Console.WriteLine("mess");
            return NoContent();
        }
    }
}