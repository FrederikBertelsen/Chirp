using System.Drawing.Printing;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ProfileModel : PageModel
{
    private readonly IAuthorRepository authorRepository;
    private readonly ICheepRepository cheepRepository;
    private readonly IFollowRepository followRepository;
    
    public List<CheepViewModel> Cheeps { get; set; }
    public List<FollowViewModel> Follows { get; set; }

    [BindProperty(SupportsGet = true)] public int currentPage { get; set; }
    public int totalPages { get; set; }
    public int navigationNumber { get; set; }
    public List<int> numbersToShow { get; set; }

    public ProfileModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository,
        IFollowRepository followRepository)
    {
        this.cheepRepository = cheepRepository;
        this.authorRepository = authorRepository;
        this.followRepository = followRepository;

        Follows = new List<FollowViewModel>();
        Cheeps = new List<CheepViewModel>();
        totalPages = 1;
        //The amount of pages that are shown between the "previous" and "next" button
        //Should always be odd, such that the current page can be in the center when relevant
        navigationNumber = 7;
        numbersToShow = new List<int>();
    }

    public async Task<IActionResult> OnGetAsync([FromQuery] int page)
    {
        if (page < 1) page = 1;

        currentPage = page;
        numbersToShow.Clear();

        IEnumerable<CheepViewModel> cheeps;
        IEnumerable<FollowViewModel> follows;
        if (User.Identity?.IsAuthenticated == true)
        {
            try
            {
                follows = await followRepository.GetFollowing(User.Identity?.Name);

                cheeps = await cheepRepository.GetPageOfCheepsByAuthor(User.Identity?.Name, page);
                totalPages = await cheepRepository.GetCheepPageAmountAuthor(User.Identity?.Name);
            }
            catch (Exception e)
            {
                cheeps = new List<CheepViewModel>();
                follows = new List<FollowViewModel>();
                Console.WriteLine(e);
            }
        }
        else
        {
            cheeps = new List<CheepViewModel>();
            follows = new List<FollowViewModel>();
        }

        Cheeps = cheeps.ToList();
        Follows = follows.ToList();

        if (currentPage - navigationNumber / 2 < 1)
            for (int i = 1; i <= navigationNumber && i <= totalPages; i++)
                numbersToShow.Add(i);
        else if (currentPage + navigationNumber / 2 > totalPages)
            for (int i = totalPages - navigationNumber; i <= totalPages; i++)
                numbersToShow.Add(i);
        else
            for (int i = currentPage - navigationNumber / 2; i <= currentPage + navigationNumber / 2; i++)
                numbersToShow.Add(i);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity?.Name is not null)
        {
            await authorRepository.RemoveAuthor(User.Identity.Name);
            return Redirect("/MicrosoftIdentity/Account/SignOut");
        }

        return RedirectToPage();
    }

    public async Task<bool> CheckFollow(string followingName)
    {
        if (User.Identity?.Name is null)
            return false;
        bool follows = await followRepository.IsFollowing(User.Identity.Name, followingName);
        return follows;
    }

    public string[] GetTaggedAuthorsFromCheepMessage(string cheepMessage) =>
        Regex.Matches(cheepMessage, @"@\(([\w -]+)\)")
            .Select(m => m.Groups[1].Value)
            .ToArray();

    public string[] SplitCheepByTags(string cheepMessage) =>
        Regex.Split(cheepMessage, @"@\([\w -]+\)");
}