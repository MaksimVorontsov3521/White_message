using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using webapi.Data;

string GetConnectionString()
{
    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string databaseFileName = "api.mdf";
    string databaseFilePath = System.IO.Path.Combine(baseDirectory, databaseFileName);

    // Строка подключения с использованием пути к файлу базы данных
    string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={databaseFilePath};Integrated Security=True;";
    return connectionString;
}

var builder = WebApplication.CreateBuilder(args);

// Настройка строки подключения
string connectionString = GetConnectionString();

// Добавляем контекст базы данных и передаем строку подключения
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Добавляем контроллеры
builder.Services.AddControllers();

// Настраиваем Kestrel для использования определённых хостов и портов
builder.WebHost.UseUrls("https://localhost:7777", "http://localhost:7201");

var app = builder.Build();

// Настройка HTTP-конвейера
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
