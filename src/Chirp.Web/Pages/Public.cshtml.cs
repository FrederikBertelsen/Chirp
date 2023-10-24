using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository cheepRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepRepository cheepRepository)
    {
        this.cheepRepository = cheepRepository;
        Cheeps = new List<CheepViewModel>();
    }

    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1) page = 1;

        try
        {
            var cheeps = cheepRepository.GetPageOfCheeps(page);
            cheeps.Wait();
            Cheeps = cheeps.Result.ToList();
        }
        catch
        {
            Cheeps = new List<CheepViewModel>();
        }

        return Page();
    }
}