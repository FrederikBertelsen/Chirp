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
    }

    public ActionResult OnGet(int? pageNo)
    {
        if (pageNo != null)
        {
            if (pageNo < 1)
            {
                pageNo = 1;
            }

            var cheeps = cheepRepository.GetPageOfCheeps(pageNo.Value);
            cheeps.Wait();
            Cheeps = cheeps.Result.ToList();
        }
        else
        {
            var cheeps = cheepRepository.GetPageOfCheeps(1);
            cheeps.Wait();
            Cheeps = cheeps.Result.ToList();
        }

        return Page();
    }
}