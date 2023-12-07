using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorRepository authorRepository;
    private readonly ICheepRepository cheepRepository;

    private readonly IFollowRepository followRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    [BindProperty(SupportsGet = true)] public int currentPage { get; set; }
    public int totalPages { get; set; }
    int pageSize { get; set; }

    public int navigationNumber { get; set; }

    public List<int> numbersToShow { get; set; }

    public string navigationAuthor { get; set; }


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository,
        IFollowRepository followRepository)
    {
        this.authorRepository = authorRepository;
        this.cheepRepository = cheepRepository;
        this.followRepository = followRepository;
        Cheeps = new List<CheepViewModel>();
        totalPages = 1;
        //The amount of pages that are shown between the "previous" and "next" button
        //Should always be odd, such that the current page can be in the center when relevant
        navigationNumber = 7;
        numbersToShow = new List<int>();
        //
        navigationAuthor = "";
    }

    public async Task<bool> CheckFollow(string followingName)
    {
        bool follows = await followRepository.IsFollowing(User.Identity?.Name, followingName);
        return follows;
    }

    public async Task<IActionResult> OnPost()
    {
        string? cheepText = Request.Form["CheepText"];
        string? userName = User.Identity?.Name;

        if (cheepText is null || userName is null || userName.Trim() == "" || cheepText.Trim() == "" ||
            cheepText.Length == 0 || cheepText.Length > 160 ||
            User.Identity?.Name is null || User.Identity.IsAuthenticated != true) return RedirectToPage();

        string cheep = cheepText;
        string authorName = userName;
        string? authorEmail = User.Claims
            .Where(c => c.Type == ClaimTypes.Email)
            .Select(c => c.Value)
            .SingleOrDefault();

        DateTime dateTime = DateTime.Now;

        Task<bool> authorTask = authorRepository.DoesUserNameExists(authorName);
        authorTask.Wait();
        bool authorExists = authorTask.Result;
        if (!authorExists)
        {
            if (authorEmail is not null)
            {
                await authorRepository.CreateAuthor(authorName, authorEmail);
            }

            await authorRepository.CreateAuthor(authorName, "noEmail@found.error");
        }

        await cheepRepository.CreateCheep(authorName, cheep, dateTime);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetAsync(string author, [FromQuery] int page)
    {
        if (page < 1) page = 1;

        currentPage = page;
        numbersToShow.Clear();

        //slightly wonky code necessary for the cshtml file to find the current author through Model.navigationAuthor
        //as it cannot directly access the author argument passed to OnGetAsync
        navigationAuthor = author;

        try
        {
            IEnumerable<CheepViewModel> cheeps;
            // IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetPageOfCheepsByAuthor(author, page);
            if (User.Identity?.IsAuthenticated == true && User.Identity?.Name == author)
            {
                cheeps = await cheepRepository.GetPageOfCheepsByFollowed(author, page);
                totalPages = await cheepRepository.GetCheepPageAmountFollowed(author);
            }
            else
            {
                cheeps = await cheepRepository.GetPageOfCheepsByAuthor(author, page);
                totalPages = await cheepRepository.GetCheepPageAmountAuthor(author);
            }

            Cheeps = cheeps.ToList();
            if (currentPage - navigationNumber / 2 < 1)
            {
                for (int i = 1; i <= navigationNumber && i <= totalPages; i++)
                {
                    numbersToShow.Add(i);
                }
            }
            else if (currentPage + navigationNumber / 2 > totalPages)
            {
                for (int i = totalPages - navigationNumber; i <= totalPages; i++)
                {
                    numbersToShow.Add(i);
                }
            }
            else
            {
                for (int i = currentPage - navigationNumber / 2; i <= currentPage + navigationNumber / 2; i++)
                {
                    numbersToShow.Add(i);
                }
            }
        }
        catch (Exception e)
        {
            //empty list of cheeps if there are no cheeps to show, this is caught by the cshtml and shown as "There are no cheeps here"
            Cheeps = new List<CheepViewModel>();
        }

        return Page();
    }

    public string AddTagLinksToCheepMessage(string cheepMessage) =>
        Regex.Replace(cheepMessage, "@\\(([\\w -]+)\\)", "<strong><a href=\"/$1\">$1</a></strong>");
}