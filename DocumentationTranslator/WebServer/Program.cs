using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Controllers;
using DocTranslatorServer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
string connectionString = Environment.GetEnvironmentVariable("DocServer_ConnectionString") ?? throw new KeyNotFoundException("Could not load environment variable: DbContext");
builder.Services.AddDbContext<DocumentContext>(opt =>
   opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<UserContext>(opt =>
   opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<LanguageContext>(opt =>
   opt.UseSqlServer(connectionString));

// Add middleware
builder.Services.AddHttpClient();
builder.Services.AddTransient<OAuthMiddleWare>();
builder.Services.AddTransient<UserDetectionMiddleWare>();

// Swagger
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
