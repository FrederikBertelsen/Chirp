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

    public IActionResult OnGet()
    {
        return Page();
    }
    //does not work yet
    public async Task<IActionResult> OnPostAsync()
    {
        string? userName = User.Identity?.Name;
        try
        {
            throw new NotImplementedException();
            //await authorRepository.RemoveAuthor(userName);
            //return RedirectToPage("/");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return RedirectToPage();
        }
    }
}