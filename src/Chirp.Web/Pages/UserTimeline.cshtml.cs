using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository cheepRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        this.cheepRepository = cheepRepository;
        Cheeps = new List<CheepViewModel>();
    }

    public ActionResult OnGet(string author, [FromQuery] int page)
    {
        if (page < 1) page = 1;

        try
        {
            var cheeps = cheepRepository.GetPageOfCheepsByAuthor(author, page);
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