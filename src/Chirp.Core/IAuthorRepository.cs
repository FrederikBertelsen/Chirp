namespace Chirp.Core;

public interface IAuthorRepository
{
    Task CreateAuthor(string authorName);
    Task RemoveAuthor(string authorName);
    Task<AuthorDTO> FindAuthorByName(string authorName);
    Task<bool> DoesUserNameExists(string authorName);
}