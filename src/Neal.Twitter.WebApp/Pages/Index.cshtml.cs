using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Neal.Twitter.WebApp.Pages;

public class IndexModel : PageModel
{
    #region Public Constructors

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public void OnGet()
    {
    }

    #endregion Public Methods

    #region Fields

    private readonly ILogger<IndexModel> _logger;

    #endregion Fields
}