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
        //assigned empty list to avoid warning/making it nullable
        Cheeps = Cheeps = new List<CheepViewModel>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
    
        if (page < 1){
            page = 1;
        }
        try{
            Cheeps = _service.GetCheeps(page);
        }
        catch {
            Cheeps = new List<CheepViewModel>();
        }
        
       
        return Page();
    }
}
