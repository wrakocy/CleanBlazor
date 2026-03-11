namespace Wrak.CleanArchitecture.Core.Shared.Interfaces;

public interface ICircuitServicesAccessor
{
    IServiceProvider? Services { get; set; }
}
