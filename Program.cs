using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NucpaBalloonsApi;
using NucpaBalloonsApi.Interfaces.Repositories;
using NucpaBalloonsApi.Interfaces.Repositories.Common;
using NucpaBalloonsApi.Interfaces.Services;
using NucpaBalloonsApi.Models.SystemModels;
using NucpaBalloonsApi.Repositories;
using NucpaBalloonsApi.Repositories.Common;
using NucpaBalloonsApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NucpaDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Add JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["AdminSettings:JwtSecret"] ?? "your-secret-key-here");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add SignalR
builder.Services.AddSignalR();

// Register services
builder.Services.AddScoped<ICodeforcesApiService, CodeforcesApiService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBalloonService, BalloonService>();
builder.Services.AddScoped<IAdminSettingsService, AdminSettingsService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();
builder.Services.AddScoped<IProblemBalloonMapService, ProblemBalloonMapService>();

builder.Services.AddScoped<IBaseRepository<AdminSettings>, BaseRepository<AdminSettings>>();
builder.Services.AddScoped<IBaseRepository<Room>, BaseRepository<Room>>();
builder.Services.AddScoped<IBaseRepository<Team>, BaseRepository<Team>>();
builder.Services.AddScoped<IBaseRepository<ProblemBalloonMap>, BaseRepository<ProblemBalloonMap>>();
builder.Services.AddScoped<IBaseRepository<AdminSettings>, BaseRepository<AdminSettings>>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();

// Configure HttpClient with longer lifetime
builder.Services.AddHttpClient<ICodeforcesApiService, CodeforcesApiService>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Disable HTTPS redirection in development
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

// Use CORS before routing
app.UseCors("AllowAll");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

