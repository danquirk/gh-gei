using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;

namespace OctoshiftCLI.Tests;

public class GithubApiBuildRulesPatternsTests
{
    [Fact]
    public async Task Includes_Pull_Request_Body_Pattern_Rule()
    {
        string capturedPayload = null;
        var client = new Mock<GithubClient>(null, null, null, null, null, "pat");
        client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<System.Collections.Generic.Dictionary<string, string>>()))
            .Callback<string, object, System.Collections.Generic.Dictionary<string, string>>((_, p, __) =>
            {
                capturedPayload = JObject.FromObject(p).ToString();
            })
            .ReturnsAsync("{\"id\":123}");

        var api = new GithubApi(client.Object, "https://api.github.com", null, null);
        var def = new GithubRulesetDefinition
        {
            Name = "rs",
            TargetPatterns = new[] { "main" },
            RequiredPullRequestBodyPatterns = new[] { "(?i)(fixes|closes) #[0-9]+" }
        };

        await api.CreateRepoRuleset("org", "repo", def);

        capturedPayload.Should().Contain("pull_request_body_pattern");
    }
}
