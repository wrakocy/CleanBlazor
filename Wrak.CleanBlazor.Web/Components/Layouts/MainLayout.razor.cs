using Wrak.CleanBlazor.Core.Shared.Interfaces;
using Wrak.CleanBlazor.Web.Interfaces;

namespace Wrak.CleanBlazor.Web.Components.Layouts;

public partial class MainLayout : LayoutComponentBase, IBrowserViewportObserver, IAsyncDisposable
{
    [Inject] private IUserContextService _userCxtService { get; set; } = default!;
    [Inject] private IAppIdentity _appId { get; set; } = default!;
    [Inject] private IAppEnvironment _appEnv { get; set; } = default!;
    [Inject] private IBrowserViewportService _viewportService { get; set; } = default!;

    public Guid Id => _observerId;

    private IUserContext? _userContext;
    private MudTheme _theme = default!;
    private bool _drawerOpen = false; // Default to closed.
    private Guid _observerId;
    private Breakpoint _currentBreakpoint = Breakpoint.None;

    public async Task NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs args)
    {
        _currentBreakpoint = args.Breakpoint;
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await InitializeUserContext();

        InitializeAppTheme();

        await InitializeViewportObserving();
    }

    private async Task InitializeUserContext()
    {
        _userContext = await _userCxtService.GetUserContextAsync();
    }

    private void InitializeAppTheme()
    {
        _theme = new MudTheme()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#2563eb",           // Vivid blue (used in primary buttons and links)
                Secondary = "#64748b",         // Tailwind slate-500 (neutral accent and secondary text)
                Background = "#f9fafb",        // Light gray-white (overall page and drawer background)
                Surface = "#ffffff",           // Pure white (cards, panels, and surfaces)

                AppbarBackground = "#16213d",  // Deep navy header (brand anchor with strong contrast)
                AppbarText = "#e2e8f0",        // Soft gray-blue text for dark header

                DrawerBackground = "#f9fafb",  // Matches page background for a seamless layout
                DrawerText = "#334155",        // Tailwind slate-700 — darker gray for good readability
                DrawerIcon = "#475569",        // Tailwind slate-600 — muted tone for balanced icons

                TextPrimary = "#0f172a",       // Main body text (slate-900)
                TextSecondary = "#64748b",     // Secondary text (slate-500)

                ActionDefault = "#64748b",     // Neutral icon/button color
                ActionDisabled = "#cbd5e1",    // Muted disabled state color (light slate)
                Divider = "#e5e7eb",           // Light neutral dividers
                TableLines = "#e2e8f0",        // Table row separators
                LinesDefault = "#e5e7eb",      // General-purpose border lines
                LinesInputs = "#cbd5e1",       // Input and search box outlines

                // Semantic accent colors
                Success = "#16a34a",           // Tailwind green-600 (affirmative / success)
                Warning = "#f59e0b",           // Tailwind amber-500 (attention / caution)
                Error = "#dc2626",             // Tailwind red-600 (error / destructive)
                Info = "#5b8df0",              // Soft vivid blue (status/info indicators)
            },

            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "185px",
            },

            Typography = new Typography()
            {
                H1 = new H1Typography()
                {
                    FontSize = "2.0em",
                    FontWeight = "600",
                },
                H2 = new H2Typography()
                {
                    FontSize = "1.8em",
                    FontWeight = "500",
                },
                H3 = new H3Typography()
                {
                    FontSize = "1.6em",
                    FontWeight = "500",
                },
                H4 = new H4Typography()
                {
                    FontSize = "1.4em",
                    FontWeight = "500",
                },
                H5 = new H5Typography()
                {
                    FontSize = "1.3em",
                    FontWeight = "600", // Bold subheading
                },
                H6 = new H6Typography()
                {
                    FontSize = "1.2em",
                    FontWeight = "600", // Bold subheading
                },

                Body1 = new Body1Typography()
                {
                    FontSize = "0.98em",
                    FontWeight = "600", // Semi-bold reading text
                },

                Body2 = new Body2Typography()
                {
                    FontSize = "0.98em",
                    FontWeight = "400", // Normal reading text
                },

                Subtitle1 = new Subtitle1Typography()
                {
                    FontSize = "1.1em",
                    FontWeight = "550", // Slightly bolder for emphasis
                    LetterSpacing = "0.01em",
                },
                Subtitle2 = new Subtitle2Typography()
                {
                    FontSize = "0.9em",
                    FontWeight = "500",
                    LetterSpacing = "0.02em",
                },

                //Caption = new CaptionTypography()
                //{
                //    // Do we want to customize caption typography?
                //},
            },
        };
    }

    private async Task InitializeViewportObserving()
    {
        _observerId = Guid.NewGuid();
        await _viewportService.SubscribeAsync(this);
    }

    private async Task TerminateViewportObserving()
    {
        if (_viewportService != null)
            await _viewportService.UnsubscribeAsync(this);
    }

    private void OnDrawerToggle() => _drawerOpen = !_drawerOpen;

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await TerminateViewportObserving();
    }
}
