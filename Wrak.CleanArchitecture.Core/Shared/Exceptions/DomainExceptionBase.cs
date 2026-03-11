namespace Wrak.CleanArchitecture.Core.Shared.Exceptions;

public abstract class DomainExceptionBase : Exception
{
    public DomainExceptionBase() { }
    public DomainExceptionBase(string message) : base(message) { }
}
