using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Neal.Twitter.WebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    #region Fields

    private readonly ILogger<ErrorModel> logger;

    #endregion Fields

    #region Properties

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    #endregion Properties

    #region Public Constructors

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        this.logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }

    #endregion Public Methods
}