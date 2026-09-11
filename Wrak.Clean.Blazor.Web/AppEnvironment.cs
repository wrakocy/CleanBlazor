using Wrak.Clean.Blazor.Core.Shared.Interfaces;

namespace Wrak.Clean.Blazor.Web;

public class AppEnvironment : IAppEnvironment
{
    private EnvironmentType _env;

    public AppEnvironment(string environment) => SetEnvironment(environment);

    public EnvironmentType GetEnvironment() => _env;
    public bool Is(EnvironmentType type) => _env == type;
    public bool IsNot(EnvironmentType type) => _env != type;

    private void SetEnvironment(string environment)
    {
        _env = environment switch
        {
            "Production" => EnvironmentType.Production,
            "Test" => EnvironmentType.Test,
            _ => EnvironmentType.Development,
        };
    }
}
