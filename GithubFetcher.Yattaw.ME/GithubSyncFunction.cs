using Amazon.Lambda.Core;

namespace GithubFetcher.Yattaw.ME;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class GithubSyncFunction
{
    
    public async Task<string> Handler(object input, ILambdaContext context)
    {
        // TODO: Fetch GitHub data and update your AWS database here.
        return "GitHub sync completed.";
    }
    
}