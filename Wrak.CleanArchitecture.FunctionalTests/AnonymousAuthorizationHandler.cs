using Microsoft.AspNetCore.Authorization;

namespace Wrak.CleanArchitecture.FunctionalTests;

public class AnonymousAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.PendingRequirements)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

