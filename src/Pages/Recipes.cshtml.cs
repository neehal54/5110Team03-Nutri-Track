using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.WebSite.Pages
{
    public class RecipesModel : PageModel
    {
        private readonly ILogger<RecipesModel> _logger;

        public RecipesModel(ILogger<RecipesModel> logger) => _logger = logger;

        public void OnGet()
        {
        }
    }
}
