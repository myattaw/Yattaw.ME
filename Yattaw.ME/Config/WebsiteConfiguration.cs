namespace Yattaw.ME.Config
{
    public class WebsiteConfiguration
    {
        public string GithubUsername { get; set; } = "myattaw";
        public string FullName { get; set; } = "Michael Yattaw";
        public List<string> Description { get; set; } = ["Fullstack Software Developer"];
        public string LinkedInUsername { get; set; } = "myattaw";

        public List<ExperienceItem> Experience { get; set; } =
        [
            new ExperienceItem
            {
                Title = "Application Developer",
                Description =
                    "Designed and optimized high-performance Minecraft server software using Java, enabling robust handling of large player counts. Skilled in developing scalable applications with a focus on performance and functionality."
            },

            new ExperienceItem
            {
                Title = "Web Developer",
                Description =
                    "Proficient in modern frameworks including React, Spring, Vue, ASP.NET, and Express, with hands-on experience building dynamic and scalable web applications."
            },

            new ExperienceItem
            {
                Title = "Amazon Web Services",
                Description = "Went through hands on AWS training and been using AWS regularly in projects."
            },

            new ExperienceItem
            {
                Title = "DevOps Engineer",
                Description = "Managed CI/CD pipelines with GitHub Actions and Docker, optimizing AWS infrastructure for scalable, high-performance applications."
            },

            new ExperienceItem
            {
                Title = "Agile Software Developer",
                Description = "Collaborated in agile teams using Kanban and paired programming to deliver high-quality code and complete sprints on time."
            },

            new ExperienceItem
            {
                Title = "AI and Data Analytics",
                Description = "Built AI models for text mining and data analysis, using web scraping and analytics methods to drive insights and improve performance."
            }
        ];
    }

    public class ExperienceItem
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string GetDescription => Description;
    }
}