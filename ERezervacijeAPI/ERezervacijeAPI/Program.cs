using ERezervacijeAPI.Data;
using ERezervacijeAPI.Helpers;
using ERezervacijeAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Twilio;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var builder = WebApplication.CreateBuilder(args); 



// Add services to the container.
builder.Services.AddDbContext<DBConnection>(options =>
    options.UseSqlServer(config.GetConnectionString("db1")));

builder.Services.AddControllers();
var smtpSettings = new SmtpSettings();
builder.Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddTransient<SmtpService>();
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddTransient<MyAuthService>();
builder.Services.AddTransient<CheckAttributesHelper>();
builder.Services.AddHttpContextAccessor();

// CORS konfiguracija
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


// Swagger konfiguracija
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "ERezervacije API",
        Description = ""
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));
}
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));


app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers(); 

app.Run();
