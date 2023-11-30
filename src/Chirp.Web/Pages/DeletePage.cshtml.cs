using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class DeleteModel : PageModel
{
    private readonly IFollowRepository followRepository;

    private readonly ICheepRepository cheepRepository;

    public List<CheepViewModel> Cheeps { get; set; }

    public DeleteModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository)
    {
        this.followRepository = followRepository;
        this.cheepRepository = cheepRepository;
    }

    public async Task<IActionResult> OnGetAsync(int CheepId)
    {
        if (User.Identity?.IsAuthenticated == true){
            IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetCheepsByAuthor(User.Identity.Name);
            foreach (CheepViewModel cheep in cheeps){
                if (cheep.CheepId == CheepId){
                    await cheepRepository.RemoveCheep(CheepId);
                }
            }
        }
        return RedirectToPage("/");
    }
}