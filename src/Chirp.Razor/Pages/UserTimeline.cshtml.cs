using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository cheepRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        this.cheepRepository = cheepRepository;
    }

    public ActionResult OnGet(string author, int? pageNo)
    {
        if (pageNo != null)
        {
            if (pageNo.Value < 1)
            {
                pageNo = 1;
            }

            var cheeps = cheepRepository.GetPageOfCheepsByAuthor(author, pageNo.Value);
            cheeps.Wait();
            Cheeps = cheeps.Result.ToList();
        }
        else
        {
            var cheeps = cheepRepository.GetPageOfCheepsByAuthor(author, 1);
            cheeps.Wait();
            Cheeps = cheeps.Result.ToList();
        }

        return Page();
    }
}
