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
            try{
                Cheeps = _service.GetCheeps(page);
            }
            catch {
                Cheeps = new List<CheepViewModel>();
            }
        }
        else {
            try{
                Cheeps = _service.GetCheeps(1);
            }
            catch {
                Cheeps = new List<CheepViewModel>();
            }
        }
        return Page();
    }
}
