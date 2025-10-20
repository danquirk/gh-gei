using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OctoshiftCLI.AdoToGithub.Commands.MigrateRepo;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;
namespace OctoshiftCLI.Tests
{
    public class MigrateRepoRulesetIntegrationTests
    {
        [Fact]
        public async Task Applies_Ruleset_When_Flag_Enabled()
        {
            CliContext.RulesetsEnabled = true;
            var log = new Mock<OctoLogger>();
            var githubApi = new Mock<GithubApi>(null, null, null, null);
            githubApi.Setup(g => g.GetOrganizationId("ghorg")).ReturnsAsync("orgid");
            githubApi.Setup(g => g.CreateAdoMigrationSource("orgid", null)).ReturnsAsync("source");
            githubApi.Setup(g => g.StartMigration("source", It.IsAny<string>(), "orgid", "repo", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync("mig1");
            githubApi.Setup(g => g.GetMigration("mig1")).ReturnsAsync(("SUCCEEDED", null, 0, null, "logurl"));
            githubApi.Setup(g => g.GetDefaultBranch("ghorg", "repo")).ReturnsAsync("main");
            githubApi.Setup(g => g.GetRepoRulesets("ghorg", "repo")).ReturnsAsync(new List<(int, string, IEnumerable<string>, int?, IEnumerable<string>)>());
            githubApi.Setup(g => g.CreateRepoRuleset("ghorg", "repo", It.IsAny<GithubRulesetDefinition>())).ReturnsAsync(99);
            var env = new Mock<EnvironmentVariableProvider>(log.Object);
            env.Setup(e => e.TargetGithubPersonalAccessToken(true)).Returns("ghpat");
            env.Setup(e => e.AdoPersonalAccessToken(true)).Returns("adopat");
            var warnings = new WarningsCountLogger(log.Object);
            var branchPolicies = new Mock<AdoBranchPolicyService>(null);
            branchPolicies.Setup(b => b.GetDefaultBranchPolicies("ado", "proj", "repo")).ReturnsAsync(System.Array.Empty<AdoPolicyConfiguration>());
            var handler = new MigrateRepoCommandHandler(log.Object, githubApi.Object, env.Object, warnings, branchPolicies.Object);
            var args = new MigrateRepoCommandArgs { AdoOrg = "ado", AdoTeamProject = "proj", AdoRepo = "repo", GithubOrg = "ghorg", GithubRepo = "repo" };
            await handler.Handle(args);
            githubApi.Verify(g => g.CreateRepoRuleset("ghorg", "repo", It.Is<GithubRulesetDefinition>(d => d.TargetPatterns.Any(x => x == "main"))), Times.Once);
        }
    }
}
