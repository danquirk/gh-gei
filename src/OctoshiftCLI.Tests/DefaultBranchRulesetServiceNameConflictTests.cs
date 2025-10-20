using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;

namespace OctoshiftCLI.Tests;

public class DefaultBranchRulesetServiceNameConflictTests
{
    [Fact]
    public async Task Creates_With_Suffix_On_Name_Conflict()
    {
        var api = new Mock<GithubApi>(null, null, null, null);
        api.Setup(a => a.GetRepoRulesets("org", "repo")).ReturnsAsync(new List<(int, string, IEnumerable<string>, int?, IEnumerable<string>)>
        {
            (10, "rs", new[] { "main" }, 1, System.Array.Empty<string>())
        });

        api.Setup(a => a.CreateRepoRuleset("org", "repo", It.IsAny<GithubRulesetDefinition>()))
            .ReturnsAsync(99)
            .Callback<string, string, GithubRulesetDefinition>((_, __, def) =>
            {
                def.Name.Should().Be("rs-1");
            });

        var log = new Mock<OctoLogger>();
        var svc = new DefaultBranchRulesetService(api.Object, log.Object);

        var def = new GithubRulesetDefinition
        {
            Name = "rs",
            TargetPatterns = new[] { "develop" },
            RequiredApprovingReviewCount = 1
        };

        var id = await svc.Apply("org", "repo", def, false);

        id.Should().Be(99);
    }
}
