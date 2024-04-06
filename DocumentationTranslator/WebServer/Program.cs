using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;
using DocTranslatorServer.Controllers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// builder.Services.AddDbContext<LanguageContext>(options =>
//      options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("Connection string 'DbContext' not found.")));
// builder.Services.AddDbContext<UserContext>(options =>
//      options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("Connection string 'DbContext' not found.")));
// builder.Services.AddDbContext<DocumentContext>(options =>
//      options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("Connection string 'DbContext' not found.")));

// Add services to the container.
builder.Services.AddDbContext<DocumentContext>(opt =>
   opt.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DbLocal") ?? throw new InvalidOperationException("local DB connection string not found!")));
builder.Services.AddDbContext<UserContext>(opt => 
   opt.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DbLocal") ?? throw new InvalidOperationException("local DB connection string not found!")));
builder.Services.AddDbContext<LanguageContext>(opt =>
   opt.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DbLocal") ?? throw new InvalidOperationException("local DB connection string not found!")));


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
