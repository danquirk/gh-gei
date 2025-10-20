using System.Linq;
using FluentAssertions;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;
namespace OctoshiftCLI.Tests; public class DefaultBranchPolicyExtractionServiceTests { [Fact] public void BuildRuleset_Aggregates_Reviewers_And_StatusChecks() { var svc = new DefaultBranchPolicyExtractionService(); var policies = new[] { new AdoPolicyConfiguration { Type = AdoPolicyType.MinimumReviewers, MinimumApproverCount = 1 }, new AdoPolicyConfiguration { Type = AdoPolicyType.MinimumReviewers, MinimumApproverCount = 3 }, new AdoPolicyConfiguration { Type = AdoPolicyType.BuildValidation, StatusCheckContext = "build" }, new AdoPolicyConfiguration { Type = AdoPolicyType.BuildValidation, StatusCheckContext = "test" } }; var def = svc.BuildRuleset("main", "ado-default-branch-policies", policies); def.RequiredApprovingReviewCount.Should().Be(3); def.RequiredStatusChecks.OrderBy(x => x).Should().Equal("build", "test"); def.TargetPatterns.Single().Should().Be("main"); } }
