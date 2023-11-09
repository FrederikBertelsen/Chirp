using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages
{
    public class PostCheepModel : PageModel
    {
        private readonly ICheepRepository cheepRepository;
        private readonly IAuthorRepository authorRepository;

        public PostCheepModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
        {
            this.cheepRepository = cheepRepository;
            this.authorRepository = authorRepository;
        }

        public async void createCheep(string userName, string cheepText)
        {
            if (userName.Trim() == "" || cheepText.Trim() == "" || User.Identity?.Name is null || User.Identity.IsAuthenticated != true) return;

            string cheep = cheepText;
            string authorName = userName;
            string? authorEmail = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value)
                .SingleOrDefault();

            // string cheep = Request.Form["text"].ToString();
            // string authorName = User.Identity.Name;
            // string? authorEmail = User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value)
            //     .SingleOrDefault();
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
        }

        public ActionResult OnGet()
        {
            return Page();
        }
    }
}