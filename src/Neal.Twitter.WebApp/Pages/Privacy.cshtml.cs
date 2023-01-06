using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Neal.Twitter.WebApp.Pages;

public class PrivacyModel : PageModel
{
    #region Public Constructors

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        this.logger = logger;
    }

    #endregion Public Constructors

    #region Public Methods

    public void OnGet()
    {
    }

    #endregion Public Methods

    #region Fields

    private readonly ILogger<PrivacyModel> logger;

    #endregion Fields
}