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

    public ActionResult OnGet(string author, int? pageNo)
    {
        if (pageNo != null){
            if (pageNo.Value < 1){
                pageNo = 1;
            }
            Cheeps = _service.GetCheepsFromAuthor(author, pageNo.Value);
        }
        else {
            Cheeps = _service.GetCheepsFromAuthor(author, 1);
        }
        return Page();
    }
}
