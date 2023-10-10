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

    public ActionResult OnGet(string author, int? page)
    {
        if (page != null){
            if (page.Value < 1){
                page = 1;
            }
            Cheeps = _service.GetCheepsFromAuthor(author, page.Value);
        }
        else {
            Cheeps = _service.GetCheepsFromAuthor(author, 1);
        }
        return Page();
    }
}
