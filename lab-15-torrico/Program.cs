using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using lab_15_torrico.Request;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Minimal API: POST /security/createtoken
app.MapPost("/security/createtoken", (UserRequest userRequest) =>
{
    if (userRequest.UserName == "jcoronel" && userRequest.Password == "123456")
    {
        var issuer = app.Configuration["Jwt:Issuer"];
        var audience = app.Configuration["Jwt:Audience"];
        var key = Encoding.UTF8.GetBytes(app.Configuration["Jwt:Key"]!);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature
        );

        var subject = new ClaimsIdentity(new[]
        {
            new Claim("Rol", "Administrador"),
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "jcoronel@example.com"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

        var expires = DateTime.UtcNow.AddMinutes(5);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Results.Ok(new { token = jwtToken });
    }

    return Results.Unauthorized();
});

app.MapControllers();

app.Run();
