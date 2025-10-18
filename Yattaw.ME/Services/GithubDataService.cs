using System.Text.RegularExpressions;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Yattaw.ME.Config;
using Yattaw.ME.Models;

namespace Yattaw.ME.Services
{
    public partial class GithubDataService
    {
        private readonly List<List<RepositoryData>> _repositoryDataList = [];
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        public GithubDataService(WebsiteConfiguration configuration, IHttpClientFactory httpClientFactory,
            ILogger<GithubDataService> logger)
        {
            var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-2";
            var config = new AmazonDynamoDBConfig { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region) };
            _dynamoDbClient = new AmazonDynamoDBClient(config);
        }

        public List<List<RepositoryData>> GetRepositoryDataList()
        {
            return _repositoryDataList;
        }

        public async Task<List<RepositoryData>> FetchRepositoryDataFromDynamoDbAsync()
        {
            var tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME");
            if (string.IsNullOrEmpty(tableName))
                throw new Exception("DYNAMODB_TABLE_NAME environment variable is not set.");

            var table = Table.LoadTable(_dynamoDbClient, tableName);
            var search = table.Scan(new ScanFilter());
            var repositoryData = new List<RepositoryData>();

            do
            {
                var documents = await search.GetNextSetAsync();
                repositoryData.AddRange(documents.Select(doc =>
                {
                    if (doc == null)
                        return RepositoryData.Create(
                            name: string.Empty,
                            description: string.Empty,
                            language: string.Empty,
                            htmlUrl: string.Empty,
                            lastUpdatedAt: DateTime.MinValue,
                            commitCount: 0,
                            starCount: 0
                        );

                    var name = string.Empty;
                    if (doc.TryGetValue("RepositoryName", out var repoNameVal) && repoNameVal != null)
                        name = repoNameVal.AsString();
                    else if (doc.TryGetValue("REPO#repoName", out var repoKeyVal) && repoKeyVal != null)
                        name = repoKeyVal.AsString()?.Replace("REPO#", "") ?? string.Empty;

                    var description = doc.ContainsKey("Description") && doc["Description"] != null
                        ? doc["Description"].AsString()
                        : string.Empty;

                    var language = doc.ContainsKey("Language") && doc["Language"] != null
                        ? doc["Language"].AsString()
                        : string.Empty;

                    var htmlUrl = doc.ContainsKey("HtmlUrl") && doc["HtmlUrl"] != null
                        ? doc["HtmlUrl"].AsString()
                        : string.Empty;

                    var readMeContent = string.Empty;
                    if (doc.TryGetValue("ReadmeContent", out var readMeVal) && readMeVal != null)
                        readMeContent = readMeVal.AsString();
                    
                    var lastUpdatedAt = DateTime.MinValue;
                    if (doc.ContainsKey("PushedAt") && doc["PushedAt"] != null)
                    {
                        var pushedAtStr = doc["PushedAt"].AsString();
                        if (!string.IsNullOrEmpty(pushedAtStr))
                        {
                            DateTime.TryParse(pushedAtStr, out lastUpdatedAt);
                        }
                    }

                    var commitCount = 0;
                    if (doc.ContainsKey("CommitCount") && doc["CommitCount"] != null)
                        commitCount = doc["CommitCount"].AsInt();

                    var starCount = 0;
                    if (doc.ContainsKey("StarCount") && doc["StarCount"] != null)
                        starCount = doc["StarCount"].AsInt();

                    return RepositoryData.Create(
                        name: name,
                        description: description,
                        language: language,
                        htmlUrl: htmlUrl,
                        lastUpdatedAt: lastUpdatedAt,
                        commitCount: commitCount,
                        starCount: starCount,
                        readMeContent
                    );
                }));
            } while (!search.IsDone);

            // Sort by CommitCount descending
            return repositoryData.OrderByDescending(r => r.CommitCount).ToList();
        }

        [GeneratedRegex(@"page=(\d+)>; rel=""last""")]
        private static partial Regex MyRegex();
    }
}