using System.Globalization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string TimeStamp);
public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        if (page != null){
            if (page < 1){
                page = 1;
            }
            Cheeps = _service.GetCheeps(page.Value);
        }
        else {
            Cheeps = _service.GetCheeps(1);
        }
        return Page();
    }
}
