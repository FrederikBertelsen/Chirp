namespace Chirp.Core;

public interface ICheepRepository
{
    Task<IEnumerable<CheepDTO>> GetPageOfCheepsByAuthor(string authorName, int pageNumber);
    
    Task<IEnumerable<CheepDTO>> GetPageOfCheepsByFollowed(string authorName, int pageNumber);

    Task<IEnumerable<CheepDTO>> GetPageOfCheeps(int pageNumber);

    Task<int> GetCheepPageAmountAll();

    Task<int> GetCheepPageAmountAuthor(string authorName);

    Task<int> GetCheepPageAmountFollowed(string authorName);

    Task<IEnumerable<int>> GetCheepIDsByAuthor(string authorName);
    Task CreateCheep(string authorName, string text);
    Task RemoveCheep(int cheepId);
}
