using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page != null){
            if (page < 1){
                page = 1;
            }
            try {
                Cheeps = _service.GetCheepsFromAuthor(author, page);
            }
            catch {
                Cheeps = new List<CheepViewModel>();
            }
        }
        else {
            try {
                Cheeps = _service.GetCheepsFromAuthor(author, 1);
            }
            catch {
                Cheeps = new List<CheepViewModel>();
            }
        }
        return Page();
    }
}
