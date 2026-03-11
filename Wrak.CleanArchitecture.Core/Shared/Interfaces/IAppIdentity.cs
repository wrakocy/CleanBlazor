namespace Wrak.CleanArchitecture.Core.Shared.Interfaces;

public interface IAppIdentity
{
    string ClientId { get; }
    string Name { get; }
}
