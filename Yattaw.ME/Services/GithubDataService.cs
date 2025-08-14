using System.Text.Json;
using System.Text.RegularExpressions;
using Yattaw.ME.Config;
using Yattaw.ME.Models;

namespace Yattaw.ME.Services
{
    public partial class GithubDataService(
        WebsiteConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GithubDataService> logger)
    {
        private const int MaxRepositoriesPerPage = 3;
        
        private readonly List<List<RepositoryData>> _repositoryDataList = [];

        public List<List<RepositoryData>> GetRepositoryDataList()
        {
            return _repositoryDataList;
        }

        private async Task<int> GetCommitCountAsync(HttpClient httpClient, string repoName)
        {
            try
            {
                // We'll use the commits endpoint with per_page=1 to get the total via Link header
                var commitsUrl = $"https://api.github.com/repos/{configuration.GithubUsername}/{repoName}/commits?per_page=1";
                logger.LogInformation("Fetching commits from {CommitsUrl}", commitsUrl);
                
                var response = await httpClient.GetAsync(commitsUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Failed to get commit count for {RepoName}: {ResponseStatusCode}", repoName, response.StatusCode);
                    return 0;
                }
                
                // Try to get the count from the Link header if it exists
                if (response.Headers.TryGetValues("Link", out var linkValues))
                {
                    var link = linkValues.FirstOrDefault();
                    if (link != null)
                    {
                        // Parse the last page number from the Link header
                        var match = MyRegex().Match(link);
                        if (match.Success && int.TryParse(match.Groups[1].Value, out var lastPage))
                        {
                            logger.LogInformation("Repository {RepoName} has approximately {LastPage} commits", repoName, lastPage);
                            return lastPage; // This is approximately the commit count
                        }
                    }
                }
                
                // If no Link header with pagination info, this likely has only one page of commits
                // Let's try to count from the current response
                var content = await response.Content.ReadAsStringAsync();
                var commits = JsonDocument.Parse(content).RootElement;
                var count = commits.GetArrayLength();
                
                if (count > 0)
                {
                    logger.LogInformation("Repository {RepoName} has {Count} commits (single page)", repoName, count);
                    return count;
                }
                
                logger.LogInformation("Repository {RepoName} has no commits", repoName);
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting commit count for {RepoName}", repoName);
                return 0;
            }
        }

        public async Task ParseRepositoryDataAsync()
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Yattaw.ME");
                
                // Get all repositories (up to 100 per page)
                var githubApi = $"https://api.github.com/users/{configuration.GithubUsername}/repos?per_page=100";
                var response = await httpClient.GetAsync(githubApi);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(content);
                var reposArray = jsonDocument.RootElement;
                
                var allRepositories = new List<RepositoryData>();
                
                // Process all repositories first
                foreach (var element in reposArray.EnumerateArray())
                {
                    var name = element.GetProperty("name").GetString() ?? "";
                    
                    var isFork = element.GetProperty("fork").GetBoolean();
                    if (isFork)
                    {
                        continue;
                    }
                    
                    var desc = element.TryGetProperty("description", out var descProp) && !descProp.ValueKind.Equals(JsonValueKind.Null) 
                        ? descProp.GetString() ?? "" 
                        : "";
                    var lang = element.TryGetProperty("language", out var langProp) && !langProp.ValueKind.Equals(JsonValueKind.Null)
                        ? langProp.GetString() ?? ""
                        : "";
                    var htmlUrl = element.GetProperty("html_url").GetString() ?? "";
                    var date = element.GetProperty("pushed_at").GetDateTime();
                    
                    // Get commit count for this repository
                    var commitCount = await GetCommitCountAsync(httpClient, name);
                    
                    allRepositories.Add(RepositoryData.Create(name, desc, lang, htmlUrl, date, commitCount));
                }
                
                logger.LogInformation("Found {AllRepositoriesCount} non-forked repositories", allRepositories.Count);
                
                // Sort repositories by commit count (descending)
                allRepositories = allRepositories.OrderByDescending(r => r.CommitCount).ToList();
                
                // Now group them into pages of MaxRepositoriesPerPage
                for (var i = 0; i < allRepositories.Count; i += MaxRepositoriesPerPage)
                {
                    var pageRepositories = allRepositories
                        .Skip(i)
                        .Take(MaxRepositoriesPerPage)
                        .ToList();
                    
                    if (pageRepositories.Count > 0)
                    {
                        _repositoryDataList.Add(pageRepositories);
                    }
                }
                
                logger.LogInformation("Created {Count} pages of repositories", _repositoryDataList.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parsing repository data");
                throw;
            }
        }

        [GeneratedRegex(@"page=(\d+)>; rel=""last""")]
        private static partial Regex MyRegex();
    }
}
