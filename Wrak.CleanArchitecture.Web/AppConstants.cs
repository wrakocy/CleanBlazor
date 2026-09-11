namespace Wrak.CleanArchitecture.Web;

public static class AppConstants
{
    public static readonly string ServiceName = "Wrak.CleanArchitecture.Web";

    public static class Card
    {
        public static readonly int Elevation = 1;
    }

    public static class Grid
    {
        public static readonly int Spacing = 5;
    }

    public static class DataGrid
    {
        public static readonly int[] PageSizeOptions = [1, 5, 10, 15, 25, 50, 100];
        public static readonly int DefaultLargeListPageSize = 15;
        public static readonly int DefaultSmallListPageSize = 10;
        public static readonly int DefaultExtraSmallListPageSize = 5;
        public static readonly bool Hover = true;
        public static readonly bool Dense = true;
        public static readonly bool Striped = true;
        public static readonly bool Bordered = true;
        public static readonly bool Outlined = true;
        public static readonly int Elevation = 0;
        public static readonly HorizontalAlignment NoRecordsAlignment = HorizontalAlignment.Center;
        public static readonly Severity NoRecordsSeverity = Severity.Normal;
        public static readonly SortMode SortMode = SortMode.Single;
        public static readonly Breakpoint Breakpoint = Breakpoint.Sm;
        public static readonly string NoRecordsClass = "mx-4";
        public static readonly string NoRecordsMessage = "No records found.";
    }

    public static class Icons
    {
        public static readonly string Home = MudBlazor.Icons.Material.Outlined.Home;
        public static readonly string About = MudBlazor.Icons.Material.Outlined.Info;
        public static readonly string Palette = MudBlazor.Icons.Material.Outlined.Palette;
        public static readonly string Typography = MudBlazor.Icons.Material.Outlined.FontDownload;
        public static readonly string Throw = MudBlazor.Icons.Material.Outlined.ErrorOutline;
        public static readonly string Routes = MudBlazor.Icons.Material.Outlined.RoundaboutLeft;
        public static readonly string Services = MudBlazor.Icons.Material.Outlined.MiscellaneousServices;
        public static readonly string Logout = MudBlazor.Icons.Material.Outlined.Logout;
        public static readonly string Help = MudBlazor.Icons.Material.Outlined.Help;

        public static readonly string BadRequest = MudBlazor.Icons.Material.Outlined.WarningAmber;
        public static readonly string Unauthorized = MudBlazor.Icons.Material.Outlined.BackHand;
        public static readonly string NotFound = MudBlazor.Icons.Material.Outlined.SearchOff;
        public static readonly string ServerError = MudBlazor.Icons.Material.Outlined.ErrorOutline;

        public static readonly string Add = MudBlazor.Icons.Material.Outlined.AddCircleOutline;
        public static readonly string Edit = MudBlazor.Icons.Material.Outlined.EditNote;
        public static readonly string Delete = MudBlazor.Icons.Material.Outlined.Delete;
        public static readonly string CopyAll = MudBlazor.Icons.Material.Outlined.CopyAll;
        public static readonly string CopyContent = MudBlazor.Icons.Material.Outlined.ContentCopy;
        public static readonly string OK = MudBlazor.Icons.Material.Outlined.CheckBox;
        public static readonly string Save = MudBlazor.Icons.Material.Outlined.Save;
        public static readonly string Cancel = MudBlazor.Icons.Material.Outlined.Cancel;
        public static readonly string Close = MudBlazor.Icons.Material.Outlined.Close;
        public static readonly string Warning = MudBlazor.Icons.Material.Outlined.Warning;
        public static readonly string Back = MudBlazor.Icons.Material.Outlined.ArrowBack;
        public static readonly string ViewDetails = MudBlazor.Icons.Material.Outlined.Search;

        public static readonly string Search = MudBlazor.Icons.Material.Filled.Search;
        public static readonly string NewSearch = MudBlazor.Icons.Material.Outlined.ManageSearch;
        public static readonly string ImageSearch = MudBlazor.Icons.Material.Outlined.ImageSearch;
        public static readonly string Clear = MudBlazor.Icons.Material.Filled.Clear;
        public static readonly string Filter = MudBlazor.Icons.Material.Filled.FilterAlt;
        public static readonly string List = MudBlazor.Icons.Material.Filled.List;
        public static readonly string CheckList = MudBlazor.Icons.Material.Filled.Checklist;
        public static readonly string Excel = MudBlazor.Icons.Custom.FileFormats.FileExcel;
        public static readonly string Info = MudBlazor.Icons.Material.Outlined.Info;
        public static readonly string CurrencyDollar = MudBlazor.Icons.Material.Outlined.AttachMoney;
    }

    public static class Breadcrumbs
    {
        public static readonly string Home = "Home";
    }

    public static class Tooltips
    {
        public static readonly int Delay = 1000; // one second
    }

    public static class Colors
    {
        public static readonly string Black = MudBlazor.Colors.Shades.Black;
        public static readonly string Blue = MudBlazor.Colors.Blue.Darken1;
        public static readonly string Orange = MudBlazor.Colors.Orange.Default;
        public static readonly string Grey = MudBlazor.Colors.Gray.Default;
        public static readonly string LightGrey = MudBlazor.Colors.Gray.Lighten1;
        public static readonly string Green = MudBlazor.Colors.Green.Darken2;
        public static readonly string Red = MudBlazor.Colors.Red.Darken1;
        public static readonly string White = MudBlazor.Colors.Shades.White;
        public static readonly string Yellow = MudBlazor.Colors.Yellow.Darken2;
        public static readonly string Brown = MudBlazor.Colors.Brown.Default;
        public static readonly string Purple = MudBlazor.Colors.Purple.Darken2;
    }

    public static class MaskPatterns
    {
        public static readonly string Wildcard = "******************************"; // up to 30 characters
    }
}
