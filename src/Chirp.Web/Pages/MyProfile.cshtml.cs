using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ProfileModel : PageModel
{
    //might not be needed
    private readonly IAuthorRepository authorRepository;
    //what is this for?
    //[BindProperty(SupportsGet = true)] 

    public ProfileModel(ICheepRepository cheepRepository,IAuthorRepository authorRepository)
    {
        this.authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
    //does not work yet
    public async Task<IActionResult> OnDelete()
    {

        return RedirectToPage("/");
    }
}