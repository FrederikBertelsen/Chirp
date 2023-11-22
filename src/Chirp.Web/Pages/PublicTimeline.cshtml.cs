using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly IAuthorRepository authorRepository;
    private readonly ICheepRepository cheepRepository;

    private readonly IFollowRepository followRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository)
    {
        this.authorRepository = authorRepository;
        this.cheepRepository = cheepRepository;
        this.followRepository = followRepository;
        Cheeps = new List<CheepViewModel>();
    }

    public async Task<bool> CheckFollow(string followingName){
        bool follows = await followRepository.IsFollowing(User.Identity?.Name, followingName);
        Console.WriteLine(followingName + " AAAAAAAAAA");
        return follows;
    }

    public async Task<IActionResult> OnPostCreateFollow(string followingName){
        Console.WriteLine(followingName + " Creating FOLLOW");
        await followRepository.AddFollower(User.Identity?.Name, followingName);
        Console.WriteLine(followingName + " Created FOLLOW");
        return RedirectToPage();
    }

     public async Task<IActionResult> OnPostDeleteFollow(string followingName){
        await followRepository.RemoveFollower(User.Identity?.Name, followingName);
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPost()
    {
        string? cheepText = Request.Form["CheepText"];
        string? userName = User.Identity?.Name;

        if (cheepText is null || userName is null || userName.Trim() == "" || cheepText.Trim() == "" || cheepText.Length == 0 || cheepText.Length > 160 ||
            User.Identity?.Name is null || User.Identity.IsAuthenticated != true) return RedirectToPage();

        string? authorEmail = User.Claims
            .Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value)
            .SingleOrDefault();
        
        DateTime dateTime = DateTime.Now;

        Task<bool> authorTask = authorRepository.DoesUserNameExists(userName);
        authorTask.Wait();
        bool authorExists = authorTask.Result;
        if (!authorExists)
        {
            if (authorEmail is not null)
            {
                await authorRepository.CreateAuthor(userName, authorEmail);
            }

            await authorRepository.CreateAuthor(userName, "noEmail@found.error");
        }

        await cheepRepository.CreateCheep(userName, cheepText, dateTime);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetAsync([FromQuery] int page)
    {
        if (page < 1) page = 1;

        try
        {
            IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetPageOfCheeps(page);
            Cheeps = cheeps.ToList();
        }
        catch
        {
            Cheeps = new List<CheepViewModel>();
        }

        return Page();
    }
}