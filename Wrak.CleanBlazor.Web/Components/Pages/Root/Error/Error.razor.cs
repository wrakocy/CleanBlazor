using System.Net;

namespace Wrak.CleanBlazor.Web.Components.Pages.Root.Error;

public class ErrorBase : AppComponentBase
{
    [Parameter, SupplyParameterFromQuery] public int? StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
    [Parameter] public Exception? Exception { get; set; }

    protected int _status;

    protected override void OnParametersSet()
    {
        _status = StatusCode ?? (int)HttpStatusCode.InternalServerError;
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var dialogParams = new DialogParameters
            {
                { nameof(ErrorModal.StatusCode), StatusCode },
                { nameof(ErrorModal.Exception), Exception }
            };

            await _dialogService.Show<ErrorModal>("", dialogParams);
        }
    }
}
