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
            // Add services to the container
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();

            // Register website configuration
            services.AddSingleton<WebsiteConfiguration>();

            // Register GitHub service
            services.AddSingleton<GithubDataService>();

            // Allow CORS for React dev server
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowReactDev");

            // ✅ Add routing BEFORE UseEndpoints
            app.UseRouting();

            // Initialize GitHub data service
            var githubService = app.ApplicationServices.GetRequiredService<GithubDataService>();
            githubService.ParseRepositoryDataAsync().GetAwaiter().GetResult();

            app.UseEndpoints(endpoints =>
            {
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

                endpoints.MapGet("/api/repositories", async context =>
                {
                    var githubService = app.ApplicationServices.GetRequiredService<GithubDataService>();
                    await context.Response.WriteAsJsonAsync(githubService.GetRepositoryDataList());
                });

                endpoints.MapGet("/api/experience", async context =>
                {
                    var config = app.ApplicationServices.GetRequiredService<WebsiteConfiguration>();
                    await context.Response.WriteAsJsonAsync(config.Experience);
                });
            });
        }
    }
}