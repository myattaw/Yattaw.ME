using System.Text.Json;
using System.Text.RegularExpressions;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
namespace GithubFetcher.Yattaw.ME;

public partial class GithubSyncFunction
{
    private static readonly HttpClient HttpClient = new();
    private static readonly string GithubApiUrl = "https://api.github.com/users/{0}/repos?per_page=100";

    public async Task<string> Handler(object input, ILambdaContext context)
    {
        var githubUsername = Environment.GetEnvironmentVariable("GITHUB_USERNAME");
        var tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME");
        if (string.IsNullOrEmpty(githubUsername))
        {
            throw new Exception("GitHub username is not configured.");
        }

        var dynamoDbClient = new AmazonDynamoDBClient();
        var table = Table.LoadTable(dynamoDbClient, tableName);

        HttpClient.DefaultRequestHeaders.Add("User-Agent", "GithubSyncFunction");
        var response = await HttpClient.GetAsync(string.Format(GithubApiUrl, githubUsername));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var repositories = JsonDocument.Parse(content).RootElement.EnumerateArray();

        foreach (var repo in repositories)
        {
            if (repo.GetProperty("fork").GetBoolean()) continue;

            var repoName = repo.GetProperty("name").GetString();
            var stars = repo.GetProperty("stargazers_count").GetInt32();

            // Fetch commit count for this repo
            var commitsApiUrl = $"https://api.github.com/repos/{githubUsername}/{repoName}/commits?per_page=1";
            var commitsReq = new HttpRequestMessage(HttpMethod.Get, commitsApiUrl);
            commitsReq.Headers.Add("User-Agent", "GithubSyncFunction");
            commitsReq.Headers.Add("Accept", "application/vnd.github.v3+json");
            var commitsResp = await HttpClient.SendAsync(commitsReq);
            commitsResp.EnsureSuccessStatusCode();

            var commitCount = 0;
            if (commitsResp.Headers.TryGetValues("Link", out var linkHeaders))
            {
                // Parse the last page number from the Link header
                var link = linkHeaders.FirstOrDefault();
                if (link != null)
                {
                    var match = MyRegex().Match(link);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out var lastPage))
                    {
                        commitCount = lastPage;
                    }
                }
            }
            else
            {
                // Only one page, so count the single commit
                var commitsJson = JsonDocument.Parse(await commitsResp.Content.ReadAsStringAsync());
                commitCount = commitsJson.RootElement.GetArrayLength();
            }

            var document = new Document
            {
                // Set the partition key as expected by your DynamoDB table
                ["REPO#repoName"] = $"REPO#{repoName}",
                ["RepositoryName"] = repoName,
                ["Description"] = repo.GetProperty("description").GetString(),
                ["Language"] = repo.GetProperty("language").GetString(),
                ["HtmlUrl"] = repo.GetProperty("html_url").GetString(),
                ["PushedAt"] = repo.GetProperty("pushed_at").GetDateTime().ToString("o"),
                ["CommitCount"] = commitCount,
                ["StarCount"] = stars
            };

            await table.PutItemAsync(document);
        }

        return "GitHub sync completed.";
    }

    [GeneratedRegex(@"[&?]page=(\d+)>; rel=""last""")]
    private static partial Regex MyRegex();
}