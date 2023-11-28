using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class FollowModel : PageModel
{
    private readonly IAuthorRepository authorRepository;
    private readonly ICheepRepository cheepRepository;

    private readonly IFollowRepository followRepository;

    public FollowModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository)
    {
        this.authorRepository = authorRepository;
        this.cheepRepository = cheepRepository;
        this.followRepository = followRepository;
    }
    public async Task<bool> CheckFollow(string followingName){
        bool follows = await followRepository.IsFollowing(User.Identity?.Name, followingName);
        return follows;
    }

    public async Task<IActionResult> OnGetAsync(string tofollow)
    {
        if (User.Identity?.IsAuthenticated == true && User.Identity.Name != tofollow){
            if (await CheckFollow(tofollow)){
                await followRepository.RemoveFollower(User.Identity?.Name, tofollow);
            }
            else {
                await followRepository.AddFollower(User.Identity?.Name, tofollow);
            }
        }
        return RedirectToPage("/");
    }
}