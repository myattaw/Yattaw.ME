using System;
using System.Globalization;

namespace Yattaw.ME.Models
{
    public class RepositoryData
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Language { get; private set; }
        public string HtmlUrl { get; private set; }
        private DateTime LastUpdatedAt { get; set; }
        public int CommitCount { get; private set; }
        public int StarCount { get; private set; }

        public RepositoryData(string name, string description, string language, string htmlUrl, DateTime lastUpdatedAt, int commitCount = 0, int starCount = 0)
        {
            Name = name;
            Description = string.IsNullOrEmpty(description) || description == "null" 
                ? "Project has no description :(" 
                : description;
            Language = string.IsNullOrEmpty(language) || language == "null" ? "None" : language;
            HtmlUrl = htmlUrl;
            LastUpdatedAt = lastUpdatedAt;
            CommitCount = commitCount;
            StarCount = starCount;
        }

        public static RepositoryData Create(string name, string description, string language, string htmlUrl, DateTime lastUpdatedAt, int commitCount = 0, int starCount = 0)
        {
            return new RepositoryData(name, description, language, htmlUrl, lastUpdatedAt, commitCount, starCount);
        }
        
        public string GetSimpleDate => LastUpdatedAt.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
    }
}
