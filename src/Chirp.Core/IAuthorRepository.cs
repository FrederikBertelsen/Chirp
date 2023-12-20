namespace Chirp.Core;
public interface IAuthorRepository
{
    Task CreateAuthor(string authorName, string authorEmail);
    Task RemoveAuthor(string authorName);
    Task<AuthorViewModel> FindAuthorByName(string authorName);
    Task<AuthorViewModel> FindAuthorByEmail(string authorEmail);
    Task<bool> DoesUserNameExists(string authorName);
    Task<bool> DoesUserEmailExists(string authorEmail);
}