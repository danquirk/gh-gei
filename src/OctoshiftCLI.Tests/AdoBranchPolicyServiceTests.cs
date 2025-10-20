using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;
namespace OctoshiftCLI.Tests
{
    public class AdoBranchPolicyServiceTests
    {
        private class TestAdoApi : AdoApi
        {
            public TestAdoApi() : base(new AdoClient(new OctoLogger(), null, null, new RetryPolicy(new OctoLogger()), "pat"), "https://dev.azure.com", new OctoLogger()) { }

            public override Task<string> GetRepoId(string org, string teamProject, string repo) => Task.FromResult("repoid");

            public override Task<JArray> GetBranchPolicyConfigurations(string org, string teamProject, string repoId, string refName)
            {
                var arr = new JArray
                {
                    new JObject
                    {
                        { "type", new JObject { { "id", "fa4e907d-c16b-4a4c-9dfa-4906e5d171dd" } } },
                        { "isEnabled", true },
                        { "settings", new JObject { { "minimumApproverCount", 3 } } }
                    },
                    new JObject
                    {
                        { "type", new JObject { { "id", "0609b952-1397-4640-95ec-e00a01b2c241" } } },
                        { "isEnabled", true },
                        { "settings", new JObject { { "displayName", "Build CI" } } }
                    }
                };
                return Task.FromResult(arr);
            }
        }

        [Fact]
        public async Task Maps_MinReviewers_And_BuildValidation()
        {
            var svc = new AdoBranchPolicyService(new TestAdoApi());
            var policies = await svc.GetDefaultBranchPolicies("org", "proj", "repo");
            policies.Count.Should().Be(2);
            policies.Should().Contain(p => p.Type == AdoPolicyType.MinimumReviewers && p.MinimumApproverCount == 3);
            policies.Should().Contain(p => p.Type == AdoPolicyType.BuildValidation && p.StatusCheckContext == "Build CI");
        }
    }
}
