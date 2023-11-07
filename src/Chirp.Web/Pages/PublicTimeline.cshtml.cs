using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository cheepRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    [BindProperty(SupportsGet = true)]
    public int currentPage {get; set;}
    int totalPages {get; set;}
    int pageSize {get; set;}

    int count {get; set;}



    public PublicModel(ICheepRepository cheepRepository)
    {
        this.cheepRepository = cheepRepository;
        Cheeps = new List<CheepViewModel>();
        pageSize = 32;
    }

    public async Task<ActionResult> OnGetAsync([FromQuery] int page)
    {
        if (page < 1) page = 1;

        currentPage = page;

        try
        {
            IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetPageOfCheeps(page);
            Cheeps = cheeps.ToList();
            count = await cheepRepository.GetCheepCountAll();
            totalPages = count/pageSize;
            if (count%pageSize != 0){
                totalPages++;
            }
        }
        catch
        {
            Cheeps = new List<CheepViewModel>();
        }

        return Page();
    }
}