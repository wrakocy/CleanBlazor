using Microsoft.JSInterop;
using Wrak.CleanArchitecture.Web.Interfaces;

namespace Wrak.CleanArchitecture.Web.Services;

public class JSInteropService : IJSInteropService
{
    private readonly IJSRuntime _js;

    public JSInteropService(IJSRuntime js)
    {
        _js = js.ThrowIfNull().Value;
    }

    public async Task DownloadSpreadsheet(string filename, byte[] bytes, CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync("downloadSpreadsheet", ct, filename, Convert.ToBase64String(bytes));
    }

    public async Task OpenNewBrowserTab(string url, CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync("openNewTab", ct, url);
    }

    public async Task ApplyMaskToElement(string elementId, string mask, object? dotNetReference, CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync("applyMaskToElement", ct, elementId, mask, dotNetReference);
    }

    public async Task ApplyDynamicLengthMaskToElement(string elementId, string lessThanOrEqualToMask, string greaterThanMask, int charThreshold, object? dotNetReference, CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync("applyDynamicLengthMaskToElement", ct, elementId, lessThanOrEqualToMask, greaterThanMask, charThreshold, dotNetReference);
    }

    public async Task SyncMaskForElementAndMoveCaret(string elementId, CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync("syncMaskForElementAndMoveCaret", ct, elementId);
    }
}
