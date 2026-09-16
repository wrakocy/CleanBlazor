using System.Net;

namespace Wrak.CleanBlazor.Web.Components.Pages.Root.Error;

public class ErrorModalBase : AppComponentBase
{
    [Parameter] public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
    [Parameter] public Exception? Exception { get; set; }

    protected string? _header;
    protected string? _message1;
    protected string? _message2;
    protected string? _detailsMessage;
    protected string? _icon;
    protected bool _expandDetails;

    protected override void OnParametersSet()
    {
        switch (StatusCode)
        {
            case (int)HttpStatusCode.BadRequest:
                {
                    _header = "Bad Request";
                    _message1 = "Something went wrong with the request you sent to the server. " +
                                "It may be missing information or have some invalid data that we failed to catch.";
                    _message2 = "Please return to the page you were working on and try your request again. If the problem continues, please contact development.";
                    _icon = AppConstants.Icons.BadRequest;
                    _detailsMessage = Exception?.Message;
                    break;
                }
            case (int)HttpStatusCode.Unauthorized:
            case (int)HttpStatusCode.Forbidden:
                {
                    _header = "Unauthorized";
                    _message1 = "You don't have permissions access the requested page or resource.";
                    _message2 = "If you think this is a mistake, please contact development.";
                    _icon = AppConstants.Icons.Unauthorized;
                    break;
                }
            case (int)HttpStatusCode.NotFound:
                {
                    _header = "Not Found";
                    _message1 = "We couldn't find the page or resource you requested. " +
                                "Maybe you entered a bad URL or resource id? Or, the page might have been removed or renamed.";
                    _message2 = "If you think this is a mistake, please contact development.";
                    _icon = AppConstants.Icons.NotFound;
                    break;
                }
            default:
                {
                    _header = "Whoops";
                    _message1 = "The server encountered an error while processing your request.";
                    _message2 = "Please try your request again. If the problem continues, please contact development.";
                    _icon = AppConstants.Icons.ServerError;
                    _detailsMessage = Exception?.Message;
                    break;
                }
        }
    }

    protected void GoHome() => _navManager.NavigateTo("/");
}
