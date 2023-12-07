using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class DeleteModel : PageModel
{
    private readonly ICheepRepository cheepRepository;

    public DeleteModel(ICheepRepository cheepRepository)
    {
        this.cheepRepository = cheepRepository;
    }

    public async Task<IActionResult> OnGetAsync(int cheepid, string redirection)
    {
        if (User.Identity?.Name is not null && User.Identity?.IsAuthenticated == true)
        {
            IEnumerable<int> ids = await cheepRepository.GetCheepIDsByAuthor(User.Identity.Name);
            foreach (int id in ids)
            {
                if (id == cheepid)
                {
                    await cheepRepository.RemoveCheep(cheepid);
                }
            }
        }

        if (redirection == "public")
            return Redirect("/");
        
        return Redirect("/" + redirection);
    }
}