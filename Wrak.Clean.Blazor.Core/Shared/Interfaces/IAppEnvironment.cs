namespace Wrak.Clean.Blazor.Core.Shared.Interfaces;

public interface IAppEnvironment
{
    EnvironmentType GetEnvironment();
    bool Is(EnvironmentType env);
    bool IsNot(EnvironmentType env);
}

public enum EnvironmentType
{
    Development,
    Test,
    Production
}
