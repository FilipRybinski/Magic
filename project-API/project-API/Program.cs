using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using project_API;
using project_API.Entities;
using project_API.Midddleware;
using project_API.Models;
using project_API.Models.Validators;
using project_API.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var authenticationSettings = new authenticationSettings();
builder.Host.UseNLog();
builder.Services.AddScoped<errorHandlingMiddleware>();
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<dataBase>();
builder.Services.AddScoped<dataSeeder>();
builder.Services.AddScoped<IAccountService, accountService>();
builder.Services.AddScoped<IPasswordHasher<user>, PasswordHasher<user>>();
builder.Services.AddScoped<IValidator<userRegisterDto>, registerUserDtoValidator>();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddCors();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});
/*builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);*/
var app = builder.Build();

var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
using (var scope = scopedFactory.CreateScope())
{
    var service=scope.ServiceProvider.GetService<dataSeeder>();
    service.Seed();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true));
app.UseMiddleware<errorHandlingMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
