namespace Chirp.Core;
public interface IAuthorRepository
{
    Task CreateAuthor(string authorName);
    Task RemoveAuthor(string authorName);
    Task<AuthorViewModel> FindAuthorByName(string authorName);
    Task<bool> DoesUserNameExists(string authorName);
}