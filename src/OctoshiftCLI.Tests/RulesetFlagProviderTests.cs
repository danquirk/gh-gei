using FluentAssertions;
using OctoshiftCLI.Services;
using Xunit;

namespace OctoshiftCLI.Tests;

public class RulesetFlagProviderTests
{
    [Theory]
    [InlineData(true, "0", true)]
    [InlineData(false, "1", true)]
    [InlineData(false, "true", true)]
    [InlineData(false, null, false)]
    public void Enabled_Resolves(bool cliArg, string envValue, bool expected)
    {
        System.Environment.SetEnvironmentVariable("OCTOSHIFT_ENABLE_RULESETS", envValue);

        var p = new RulesetFlagProvider(null);
        p.Enabled(cliArg).Should().Be(expected);

        System.Environment.SetEnvironmentVariable("OCTOSHIFT_ENABLE_RULESETS", null);
    }
}
