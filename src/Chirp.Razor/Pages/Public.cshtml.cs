using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string TimeStamp);

public class PublicModel : PageModel
{
    private readonly ICheepService _service;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public List<CheepViewModel> Cheeps { get; set; }

    public ActionResult OnGet()
    {
        Cheeps = _service.GetCheeps();
        return Page();
    }
}