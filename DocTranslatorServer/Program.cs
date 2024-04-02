using Microsoft.EntityFrameworkCore;
using DocTranslatorServer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DocumentsContext>(opt =>
    opt.UseInMemoryDatabase("DocumentList"));


builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub";
})
.AddCookie()
.AddOAuth("GitHub", options =>
{
    options.ClientId = "myID";
    options.ClientSecret = "mySecret";
    // TODO: Add the rest here.
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
