using Yattaw.ME.Config;
using Yattaw.ME.Services;

namespace Yattaw.ME
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();  // 👈 Add this line first

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();

            services.AddSingleton<WebsiteConfiguration>();
            services.AddSingleton<GithubDataService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDev", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",
                            "https://api.yattaw.me"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // 🚨 ORDER MATTERS
            app.UseRouting();                 // 1️⃣ Add this before CORS
            app.UseCors("AllowReactDev");     // 2️⃣ CORS goes after UseRouting
            app.UseAuthorization();           // (optional but standard)
            
            app.UseEndpoints(endpoints =>
            {
                // GET /api/profile
                endpoints.MapGet("/api/profile", async context =>
                {
                    var config = app.ApplicationServices.GetRequiredService<WebsiteConfiguration>();
                    await context.Response.WriteAsJsonAsync(new
                    {
                        fullName = config.FullName,
                        description = string.Join(" ", config.Description),
                        githubUrl = $"https://github.com/{config.GithubUsername}/",
                        linkedinUrl = $"https://www.linkedin.com/in/{config.LinkedInUsername}/"
                    });
                });

                // GET /api/repositories
                endpoints.MapGet("/api/repositories", async context =>
                {
                    var githubService = app.ApplicationServices.GetRequiredService<GithubDataService>();
                    var repositories = await githubService.FetchRepositoryDataFromDynamoDbAsync();
                    await context.Response.WriteAsJsonAsync(repositories);
                });

                // GET /api/experience
                endpoints.MapGet("/api/experience", async context =>
                {
                    var config = app.ApplicationServices.GetRequiredService<WebsiteConfiguration>();
                    await context.Response.WriteAsJsonAsync(config.Experience);
                });
            });
        }
    }
}