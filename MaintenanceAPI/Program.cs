using System.Text;
using MaintenanceAPI.Data;
using MaintenanceAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<MaintenanceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MaintenanceDbContext>();

                if (!db.Users.Any())
                {
                    var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                    var techPassword = BCrypt.Net.BCrypt.HashPassword("Tech@123");
                    var userPassword = BCrypt.Net.BCrypt.HashPassword("User@123");

                    var users = new List<User>
                    {
                        new User { Name = "Admin User", Email = "admin@demo.com", PasswordHash = adminPassword, Role = "Admin" },
                        new User { Name = "Technician One", Email = "tech@demo.com", PasswordHash = techPassword, Role = "Technician" },
                        new User { Name = "Resident User", Email = "user@demo.com", PasswordHash = userPassword, Role = "Resident" }
                    };

                    db.Users.AddRange(users);
                    db.SaveChanges();
                }
            }
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();
