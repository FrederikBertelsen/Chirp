using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

//tests that should be reevaluated when pages is nolonger a field in Cheep Reposetory-object:

namespace test;

public class CheepRepositoryTest
{
    readonly ChirpDBContext _context;

    //might not be neccarcary when pageSize is refactored
    readonly int _pageSize;

    /// <summary>
    /// The setup for cheeprepo-test, here the connection to the database and the dbContext is created 
    /// </summary>
    public CheepRepositoryTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var contextOptions = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection).Options;
        _context = new ChirpDBContext(contextOptions);
        _context.Database.EnsureCreated();
        _pageSize = 32;
        SeedDatabase(_context);
    }

    /// <summary>
    /// method for seeding the database with data for testing
    /// </summary>
    /// <param name="context"> the context dbcontext for testing</param>
    void SeedDatabase(ChirpDBContext context)
    {
        var a1 = new Author() { AuthorId = 1, Name = "existingAuthor", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Luanna Muro", Cheeps = new List<Cheep>() };
        var a3 = new Author() { AuthorId = 3, Name = "Filip Muro", Cheeps = new List<Cheep>() };


        var authors = new List<Author>() { a1, a2 };

        var c1 = new Cheep()
        {
            CheepId = 1, Author = a1,
            Text =
                "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.",
            TimeStamp = DateTime.Parse("2023-08-01 13:14:37")
        };
        var c2 = new Cheep()
        {
            CheepId = 2, Author = a1, Text = "And then, as he listened to all that''s left o'' twenty-one people.",
            TimeStamp = DateTime.Parse("2023-08-01 13:15:21")
        };
        var c3 = new Cheep()
        {
            CheepId = 3, Author = a2, Text = "In various enchanted attitudes, like the Sperm Whale.",
            TimeStamp = DateTime.Parse("2023-08-01 13:14:58")
        };
        var c4 = new Cheep()
        {
            CheepId = 4, Author = a1,
            Text =
                "Unless we succeed in establishing ourselves in some monomaniac way whatever significance might lurk in them.",
            TimeStamp = DateTime.Parse("2023-08-01 13:14:34")
        };
        var c5 = new Cheep()
        {
            CheepId = 5, Author = a2, Text = "At last we came back!", TimeStamp = DateTime.Parse("2023-08-01 13:14:35")
        };
        var c6 = new Cheep()
        {
            CheepId = 6, Author = a3, Text = "I like the cheese.", TimeStamp = DateTime.Parse("2023-08-01 13:16:34")
        };
        var c7 = new Cheep()
        {
            CheepId = 7, Author = a3, Text = "At last we came back to cheese!",
            TimeStamp = DateTime.Parse("2023-08-01 13:18:35")
        };

        var cheeps = new List<Cheep>() { c1, c2, c3, c4, c5, c6, c7 };

        context.Follows.Add(new Follow()
            { Follower = a1, FollowerId = a1.AuthorId, Following = a1, FollowingId = a1.AuthorId });
        context.Follows.Add(new Follow()
            { Follower = a2, FollowerId = a2.AuthorId, Following = a2, FollowingId = a2.AuthorId });
        context.Follows.Add(new Follow()
            { Follower = a3, FollowerId = a3.AuthorId, Following = a3, FollowingId = a3.AuthorId });

        context.Follows.Add(new Follow()
            { Follower = a3, FollowerId = a3.AuthorId, Following = a2, FollowingId = a2.AuthorId });
        context.Follows.Add(new Follow()
            { Follower = a3, FollowerId = a3.AuthorId, Following = a1, FollowingId = a1.AuthorId });

        context.Follows.Add(new Follow()
            { Follower = a2, FollowerId = a2.AuthorId, Following = a3, FollowingId = a3.AuthorId });


        a1.Cheeps = new List<Cheep>() { c1, c2, c4 };
        a2.Cheeps = new List<Cheep>() { c3, c5 };
        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
    }

    /// <summary>
    /// Checks that the GetPageOfCheeps method returns the correct amount
    /// </summary>
    [Fact]
    public async void GetPageOfCheeps()
    {
        //arrange
        int actualCount = 0;
        CheepRepository cr = new CheepRepository(_context);

        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheeps(1);
        foreach (var e in result)
            actualCount++;

        //assert
        Assert.Equal(Math.Min(_context.Cheeps.Count(), _pageSize), actualCount);
    }

    /// <summary>
    /// test the method GetPageOfCheeps if an illegal pagenumber is geven as input
    /// </summary>
    /// <param name="pageNumber">pageNumber input for GetPageOfCheeps method</param>
    [Theory]
    [InlineData(0)]
    [InlineData(-6)]
    public async void GetPageOfCheepsPageNumberOutOfRange(int pageNumber)
    {
        //arrange
        CheepRepository cr = new CheepRepository(_context);

        //act
        async Task result() => await cr.GetPageOfCheeps(pageNumber);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }

    /// <summary>
    /// Test of successfully creating a cheep
    /// </summary>
    /// <param name="author">The index of the author in the list of authors created during database setup</param>
    /// <param name="text">Text of the created cheep</param>
    [Theory]
    [InlineData("existingAuthor", "Hello")]
    [InlineData("existingAuthor",
        "this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. ")]
    [InlineData("existingAuthor", "1")]
    public async void CreateCheep(string author, string text)
    {
        //arrange
        int cheepCount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);

        //act
        await cr.CreateCheep(author, text);

        //assert
        Assert.Equal(cheepCount + 1, _context.Cheeps.Count());
    }

    /// <summary>
    /// Tests that the correct exeption is thrown if any or all values of data in a cheep is null
    /// </summary>
    [Fact]
    public async void CreateCheepWithNullValues()
    {
        //arrange
        string? text = null;
        int cheepCount = _context.Cheeps.Count();
        string author = "existingAuthor";
        CheepRepository cr = new CheepRepository(_context);

        //act
        async Task result() => await cr.CreateCheep(author, text);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepCount, _context.Cheeps.Count());
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateCheep method 
    /// is called with a nonExisting author
    /// </summary>
    [Fact]
    public async void CreateCheepAuthorDoesNotExist()
    {
        //arrange
        string authorName = "nonExisitngAuthor";
        int cheepcount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);
        string text = "Hello";

        //act
        async Task result() => await cr.CreateCheep(authorName, text);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, _context.Cheeps.Count());
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateCheep 
    /// method is called with a null value as author
    /// </summary>
    [Fact]
    public async void CreateCheepAuthorNull()
    {
        //arrange
        string? authorName = null;
        int cheepcount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);
        string text = "Hello";

        //act
        async Task result() => await cr.CreateCheep(authorName, text);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepcount, _context.Cheeps.Count());
    }

    /// <summary>
    /// Testing that an argumentExeption is thrown if the length of
    /// the input-text exeeds the requirements of 1-160 chars
    /// </summary>
    /// <param name="text">textfield in CreateCheep mehtod, s
    /// hould not have a length between 1-160</param>
    [Theory]
    [InlineData("")]
    [InlineData(
        "this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars..")]
    public async void CreateCheepExtendingLimits(string text)
    {
        //arrange
        int cheepCount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);
        string author = "existingAuthor";

        //act
        async Task result() => await cr.CreateCheep(author, text);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepCount, _context.Cheeps.Count());
    }

    /// <summary>
    /// test that the RemoveCheep method functions correctly
    /// </summary>
    /// <param name="cheepId">Id of the cheep to be removed</param>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async void RemoveCheep(int cheepId)
    {
        //arrange
        int cheepCount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);

        //act
        await cr.RemoveCheep(cheepId);

        //assert
        Assert.Equal(cheepCount - 1, _context.Cheeps.Count());
    }

    /// <summary>
    /// tests what happens if a non existing cheep is attempted removed
    /// </summary>
    /// <param name="cheepId">Id of nonexisting cheep</param>
    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public async void RemoveCheepDoesNotExist(int cheepId)
    {
        //arrange
        int cheepcount = _context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(_context);

        //act
        async Task result() => await cr.RemoveCheep(cheepId);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, _context.Cheeps.Count());
    }

    //cannot be made properly before pagesize can be decided by methodcall
    //only works because we do not have more than 32 cheeps per author in database
    /// <summary>
    /// tests that the GetCheepsByAuthor method fuctions propperly
    /// </summary>
    /// <param name="author">author name</param>
    /// <param name="cheepCountByAuthor">cheeps in the database by that author</param>
    [Theory]
    [InlineData("existingAuthor", 3)]
    [InlineData("Luanna Muro", 2)]
    public async void GetCheepsByAuthor(string author, int cheepCountByAuthor)
    {
        //arrange
        CheepRepository cr = new CheepRepository(_context);
        int actualCount = 0;

        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheepsByAuthor(author, 1);
        foreach (var e in result)
            actualCount++;

        //assert
        Assert.Equal(actualCount, cheepCountByAuthor);
    }


    /// <summary>
    /// test that an ArgumentExeption is thrown when calling the GetCheepsByAuthor method
    /// with an author name not in the database
    /// </summary>
    [Fact]
    public async void GetCheepsByNonExistingAuthor()
    {
        //arrange
        string authorName = "nonExisitngAuthor";
        CheepRepository cr = new CheepRepository(_context);

        //act
        async Task result() => await cr.GetPageOfCheepsByAuthor(authorName, 1);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }

    /// <summary>
    /// test that an ArgumentNullExeption is thrown when calling the GetCheepsByAuthor method
    /// with an author name not in the database
    /// </summary>
    [Fact]
    public async void GetCheepsByNullAuthor()
    {
        //arrange
        string? authorName = null;
        CheepRepository cr = new CheepRepository(_context);

        //act
        async Task result() => await cr.GetPageOfCheepsByAuthor(authorName, 1);

        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
    }

    /// <summary>
    /// Checks that my timline contains the right amount of cheeps when folloing other users.
    /// </summary>
    /// <param name="author">the author whos timline we are checking</param>
    /// <param name="cheepCountByFolloews">how many cheeps the individual my timline contains</param>
    [Theory]
    [InlineData("Filip Muro", 7)]
    [InlineData("Luanna Muro", 4)]
    public async void GetCheepsByFollows(string author, int cheepCountByFolloews)
    {
        //arrange
        CheepRepository cr = new CheepRepository(_context);
        int actualCount = 0;

        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheepsByFollowed(author, 1);
        foreach (var e in result)
            actualCount++;

        //assert
        Assert.Equal(actualCount, cheepCountByFolloews);
    }

    /// <summary>
    ///  checks that a user that does not follow anyone only sees there own cheeps on my timline
    /// </summary>
    [Fact]
    public async void GetCheepsByFollowsWithNoFollows()
    {
        //arrange
        string authorName = "existingAuthor";
        int cheepCountByFollows = 3;
        CheepRepository cr = new CheepRepository(_context);
        int actualCount = 0;

        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheepsByFollowed(authorName, 1);
        foreach (var e in result)
            actualCount++;

        //assert
        Assert.Equal(actualCount, cheepCountByFollows);
    }
}