using System.Linq;
using FluentAssertions;
using OctoshiftCLI.Models;
using OctoshiftCLI.Services;
using Xunit;
namespace OctoshiftCLI.Tests; public class DefaultBranchPolicyExtractionServiceStatusLimitTests { [Fact] public void Truncates_Status_Checks_To_50() { var svc = new DefaultBranchPolicyExtractionService(); var policies = Enumerable.Range(0, 60).Select(i => new AdoPolicyConfiguration { Type = AdoPolicyType.BuildValidation, StatusCheckContext = $"check-{i}" }); var def = svc.BuildRuleset("main", "rs", policies); def.RequiredStatusChecks.Count.Should().Be(50); def.RequiredStatusChecks[0].Should().Be("check-0"); def.RequiredStatusChecks[def.RequiredStatusChecks.Count - 1].Should().Be("check-49"); } }
