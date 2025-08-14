using Yattaw.ME.Config;
using Yattaw.ME.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Register our website configuration
builder.Services.AddSingleton<WebsiteConfiguration>();

// Register GitHub service
builder.Services.AddSingleton<GithubDataService>();

// Allow CORS for React dev server - more permissive for testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactDev");

// Initialize GitHub data service
var githubService = app.Services.GetRequiredService<GithubDataService>();
await githubService.ParseRepositoryDataAsync();

// These are minimal API endpoints - defined directly in Program.cs without controllers
app.MapGet("/api/profile", (WebsiteConfiguration config) => new {
    fullName = config.FullName,
    description = string.Join(" ", config.Description),
    githubUrl = $"https://github.com/{config.GithubUsername}/",
    linkedinUrl = $"https://www.linkedin.com/in/{config.LinkedInUsername}/"
});

app.MapGet("/api/repositories", (GithubDataService githubService) => githubService.GetRepositoryDataList());

app.MapGet("/api/experience", (WebsiteConfiguration config) => config.Experience);

app.Run();
