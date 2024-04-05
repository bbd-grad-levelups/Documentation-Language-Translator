using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;
using DocTranslatorServer.Controllers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<LanguageContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("LanguageContext") ?? throw new InvalidOperationException("Connection string 'LanguageContext' not found.")));
// builder.Services.AddDbContext<UserContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("UserContext") ?? throw new InvalidOperationException("Connection string 'UserContext' not found.")));
// builder.Services.AddDbContext<DocumentContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DocumentContext") ?? throw new InvalidOperationException("Connection string 'DocumentContext' not found.")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DocumentContext>(opt =>
    opt.UseInMemoryDatabase("localDB"));
builder.Services.AddDbContext<UserContext>(opt => 
    opt.UseInMemoryDatabase("localDB"));
builder.Services.AddDbContext<LanguageContext>(opt =>
    opt.UseInMemoryDatabase("localDB"));


// Add middleware
builder.Services.AddHttpClient();
builder.Services.AddTransient<OAuthMiddleWare>();
builder.Services.AddTransient<UserDetectionMiddleWare>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseMiddleware<OAuthMiddleWare>();
app.UseMiddleware<UserDetectionMiddleWare>();

app.MapControllers();

app.Run();
