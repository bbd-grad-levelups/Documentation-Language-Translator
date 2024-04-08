using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Controllers;
using DocTranslatorServer.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddDbContext<DocumentContext>(opt =>
   opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("local DB connection string not found!")));
builder.Services.AddDbContext<UserContext>(opt =>
   opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("local DB connection string not found!")));
builder.Services.AddDbContext<LanguageContext>(opt =>
   opt.UseSqlServer(builder.Configuration.GetConnectionString("DbContext") ?? throw new InvalidOperationException("local DB connection string not found!")));

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
