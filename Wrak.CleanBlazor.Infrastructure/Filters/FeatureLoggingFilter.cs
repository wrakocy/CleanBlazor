using System.Diagnostics;
using Wrak.CleanBlazor.Core.Shared.Interfaces;

namespace Wrak.CleanBlazor.Infrastructure.Filters;

public class FeatureLoggingFilter<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUserContext _userContext;

    public FeatureLoggingFilter(IUserContext userContext)
    {
        _userContext = userContext.ThrowIfNull().Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Excplicity ignore logging certain feature invocations. 
        if (request is not INoFeatureLogging)
        {
            // Log the request details.     
            Log.Information($"Handling {typeof(TRequest).Name} for {_userContext.UserName}.");
            var watch = new Stopwatch();

            watch.Start();
            var response = await next();
            watch.Stop();

            // Return the reponse.
            Log.Information($"Handled {typeof(TRequest).Name} for {_userContext.UserName} (time elapsed: {watch.ElapsedMilliseconds}ms)");
            return response;
        }
        else
            return await next();
    }
}
