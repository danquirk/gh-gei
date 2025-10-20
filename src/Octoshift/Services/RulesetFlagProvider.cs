namespace OctoshiftCLI.Services;

public class RulesetFlagProvider
{
    private readonly EnvironmentVariableProvider _env;

    public RulesetFlagProvider(EnvironmentVariableProvider env)
    {
        _env = env;
    }

    public bool Enabled(bool cliArg)
    {
        var envFlag = System.Environment.GetEnvironmentVariable("OCTOSHIFT_ENABLE_RULESETS");
        return cliArg || envFlag?.ToUpperInvariant() is "TRUE" or "1";
    }
}
