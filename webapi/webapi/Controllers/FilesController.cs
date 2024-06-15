using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using webapi.Controllers;
using webapi.Data;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public FilesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
        _context = context;
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] int messageId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.Combine(_uploadPath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        //Обновите сообщение, чтобы указать файл
        var message = await _context.Message.FindAsync(messageId);
        if (message == null)
            return NotFound("Message not found.");

        message.FileName = file.FileName;
        message.FilePath = filePath;
        message.FileType = file.ContentType;

        await _context.SaveChangesAsync();
        return Ok(new { message.FileName, message.FilePath, message.FileType });
    }
    //[HttpPost("upload-file")]
    //public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //    {
    //        return BadRequest("No file uploaded or file is empty.");
    //    }

    //    try
    //    {
    //        var uploadPath = Path.Combine(_environment.ContentRootPath, "Uploads");
    //        if (!Directory.Exists(uploadPath))
    //        {
    //            Directory.CreateDirectory(uploadPath);
    //        }

    //        var filePath = Path.Combine(uploadPath, file.FileName);

    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        return Ok(new { FileName = file.FileName, FilePath = filePath });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}

    //public async Task<byte[]> DownloadFileFromDatabaseAsync(int messageId)
    //{
    //    // Предположим, что вы используете Entity Framework Core для работы с базой данных
    //    var message = await _context.Message.FindAsync(messageId);
    //    if (message == null || string.IsNullOrEmpty(message.FilePath))
    //    {
    //        throw new FileNotFoundException("File not found in the database.");
    //    }

    //    // Чтение содержимого файла в байтовый массив
    //    return await File.ReadAllBytesAsync(message.FilePath);
    //}

    //[HttpGet("download/{id}")]
    //public async Task<IActionResult> DownloadFile(int messageId)
    //{
    //    try
    //    {
    //        var fileBytes = await DownloadFileFromDatabaseAsync(messageId);
    //        if (fileBytes == null || fileBytes.Length == 0)
    //        {
    //            return NotFound("File not found.");
    //        }

    //        // Получение сообщения из базы данных (если требуется)
    //        var message = await _context.Message.FindAsync(messageId);
    //        if (message == null)
    //        {
    //            return NotFound("Message not found.");
    //        }

    //        // Возвращаем файл клиенту с указанием MIME-типа
    //        return File(fileBytes, message.FileType, message.FileName);
    //    }
    //    catch (FileNotFoundException)
    //    {
    //        return NotFound("File not found in the database.");
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}
    //public async Task<IActionResult> DownloadFile(int id)
    //{
    //    var message = await _context.Message.FindAsync(id);
    //    if (message == null || string.IsNullOrEmpty(message.FilePath))
    //        return NotFound("File not found.");

    //    var memory = new MemoryStream();
    //    using (var stream = new FileStream(message.FilePath, FileMode.Open))
    //    {
    //        await stream.CopyToAsync(memory);
    //    }
    //    memory.Position = 0;
    //    return File(memory, message.FileType, message.FileName);
    //}
}
