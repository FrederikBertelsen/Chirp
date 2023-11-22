using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorRepository authorRepository;
    private readonly ICheepRepository cheepRepository;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        this.authorRepository = authorRepository;
        this.cheepRepository = cheepRepository;
        Cheeps = new List<CheepViewModel>();
    }
    public async Task<IActionResult> OnPost()
    {
        string? cheepText = Request.Form["CheepText"];
        string? userName = User.Identity?.Name;

        if (cheepText is null || userName is null || userName.Trim() == "" || cheepText.Trim() == "" || cheepText.Length == 0 || cheepText.Length > 160 ||
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

        try
        {
            // IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetPageOfCheepsByAuthor(author, page);
            IEnumerable<CheepViewModel> cheeps = await cheepRepository.GetPageOfCheepsByFollowed(author, page);
            Cheeps = cheeps.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e);
            Cheeps = new List<CheepViewModel>();
        }

        return Page();
    }
}