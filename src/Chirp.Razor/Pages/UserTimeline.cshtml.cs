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
        //assigned empty list to avoid warning/making it nullable
        Cheeps = Cheeps = new List<CheepViewModel>();
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        
        if (page < 1){
            page = 1;
        }
        try {
            Cheeps = _service.GetCheepsFromAuthor(author, page);
        }
        catch {
            Cheeps = new List<CheepViewModel>();
        }
        
        return Page();
    }
}
