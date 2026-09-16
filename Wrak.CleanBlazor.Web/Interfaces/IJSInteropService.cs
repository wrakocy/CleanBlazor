namespace Wrak.CleanBlazor.Web.Interfaces;

public interface IJSInteropService
{
    Task DownloadSpreadsheet(string filename, byte[] bytes, CancellationToken ct = default);
    Task OpenNewBrowserTab(string url, CancellationToken ct = default);
    Task ApplyMaskToElement(string elementId, string mask, object? dotNetReference, CancellationToken ct = default);
    Task ApplyDynamicLengthMaskToElement(string elementId, string lessThanOrEqualToMask, string greaterThanMask, int charThreshold, object? dotNetReference, CancellationToken ct = default);
    Task SyncMaskForElementAndMoveCaret(string elementId, CancellationToken ct = default);
}
