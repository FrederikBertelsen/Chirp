using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public interface ICheepRepository
{
    IEnumerable<Cheep> GetCheepsByAuthor(string authorName);

    IEnumerable<Cheep> GetPageOfCheepsByAuthor(string authorName, int pageNumber);
    IEnumerable<Cheep> GetAllCheeps();

    IEnumerable<Cheep> GetPageOfCheeps(int pageNumber);
    void AddCheep(string authorName, string message, DateTime timestamp);
    void RemoveCheep(int cheepId);
}

public class CheepRepository : ICheepRepository
{
    private const int PageSize = 32;

    private readonly ChirpDBContext dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<Cheep> GetCheepsByAuthor(string authorName)
    {
        Author author = dbContext.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .Single();

        List<Cheep> orderedCheeps = author.Cheeps.OrderBy(c => c.TimeStamp).ToList();
        return orderedCheeps;
    }

    public IEnumerable<Cheep> GetPageOfCheepsByAuthor(string authorName, int pageNumber)
    {
        int skipCount = (pageNumber - 1) * PageSize;
        
        Author author = dbContext.Authors
            .Where(a => a.Name == authorName)
            .Include(a => a.Cheeps)
            .Single();

        List<Cheep> orderedCheeps = author.Cheeps.OrderBy(c => c.TimeStamp)
            .Skip(skipCount)
            .Take(PageSize)
            .ToList();
        return orderedCheeps;
    }

    public IEnumerable<Cheep> GetAllCheeps()
    {
        List<Cheep> dbCheepList = dbContext.Cheeps
            .Include(c => c.Author)
            .OrderBy(c => c.TimeStamp)
            .ToList();

        return dbCheepList;
    }

    public IEnumerable<Cheep> GetPageOfCheeps(int pageNumber)
    {
        int skipCount = (pageNumber - 1) * PageSize;

        List<Cheep> dbCheepList = dbContext.Cheeps
            .Skip(skipCount)
            .Take(PageSize)
            .ToList();

        return dbCheepList;
    }

    public void AddCheep(string authorName, string message, DateTime timestamp)
    {
        Author author = dbContext.Authors.Single(a => a.Name == authorName);
        Cheep newCheep = new Cheep() { Author = author, Text = message, TimeStamp = timestamp };
        dbContext.Cheeps.Add(newCheep);
        dbContext.SaveChanges();
    }

    public void RemoveCheep(int cheepId)
    {
        Cheep cheep = dbContext.Cheeps.Single(c => c.CheepId == cheepId);
        dbContext.Cheeps.Remove(cheep);
    }
}