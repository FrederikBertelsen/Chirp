using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test;

public class AuthorRepositoryTest
{
    readonly ChirpDBContext _context;

    /// <summary>
    /// The setup for Author repo-test, here the connection to the database and the dbContext is created 
    /// </summary>
    public AuthorRepositoryTest()
    {
        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        DbContextOptions<ChirpDBContext> contextOptions =
            new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection).Options;
        _context = new ChirpDBContext(contextOptions);
        _context.Database.EnsureCreated();
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

        var cheeps = new List<Cheep>() { c1, c2, c3, c4, c5 };

        a1.Cheeps = new List<Cheep>() { c1, c2, c4 };
        a2.Cheeps = new List<Cheep>() { c3, c5 };
        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
    }

    /// <summary>
    /// tests that different ways of creating an author executes correctly 
    /// </summary>
    /// <param name="authorName"></param>
    [Theory]
    //basic format
    [InlineData("someName")]
    //name with space in 
    [InlineData("some Name")]
    //name with special characters
    [InlineData("some_Name?!\\")]
    public async void CreateNonexistingAuthor(string authorName)
    {
        //arrange
        AuthorRepository authorRepository = new(_context);

        //act
        int size = _context.Authors.Count();
        await authorRepository.CreateAuthor(authorName);

        //assert
        Assert.Equal(size + 1, _context.Authors.Count());
        Assert.NotNull(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
    }

    /// <summary>
    /// creates author with data already in the database
    /// </summary>
    [Fact]
    public async void CreateAuthorsWithExistingData()
    {
        //arrange
        string authorName = "existingAuthor";
        AuthorRepository authorRepository = new(_context);

        //act
        int size = _context.Authors.Count();
        async Task result() => await authorRepository.CreateAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(size, _context.Authors.Count());
        Assert.NotNull(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateAuthor 
    /// method is called with a null values
    /// </summary>
    [Fact]
    public async void CreateAuthorNullValue()
    {
        //arrange
        string? authorName = null;
        AuthorRepository authorRepository = new(_context);

        //act
        int size = _context.Authors.Count();
        async Task result() => await authorRepository.CreateAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<DbUpdateException>(result);
        Assert.Equal(size, _context.Authors.Count());
        Assert.Null(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateAuthor 
    /// method is called twice in a row, witch would otherwise create dublicate authors
    /// </summary>
    [Fact]
    public async void CreateAuthorTwiceTest()
    {
        //arrange
        string authorName = "someName";
        AuthorRepository authorRepository = new(_context);

        //act
        int size = _context.Authors.Count();
        await authorRepository.CreateAuthor(authorName);
        async Task result() => await authorRepository.CreateAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(size + 1, _context.Authors.Count());
        Assert.NotNull(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
    }

    /// <summary>
    /// tests that removing an author that exists in the database works correctly
    /// </summary>
    [Fact]
    public async void RemoveExistingAuthorTest()
    {
        //arrange
        string? authorName = "existingAuthor";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        int size = _context.Authors.Count();
        await authorRepository.RemoveAuthor(authorName);

        //assert
        Assert.Equal(size - 1, _context.Authors.Count());
        Assert.Null(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
        Assert.False(_context.Cheeps.Any(c => c.Author.Name == authorName));
        Assert.False(_context.Follows.Any(f => f.Follower.Name == authorName || f.Following.Name == authorName));
    }

    /// <summary>
    /// tests that removing an author with null as the name value returns an error
    /// </summary>
    [Fact]
    public async void RemoveAuthorNullValue()
    {
        //arrange
        string? authorName = null;
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        async Task result() => await authorRepository.RemoveAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
    }

    /// <summary>
    /// tests that removing an author that exists in the database twice is no different from removing them once
    /// </summary>
    [Fact]
    public async void RemoveAuthorTwice()
    {
        //arrange
        string authorName = "existingAuthor";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        int size = _context.Authors.Count();
        await authorRepository.RemoveAuthor(authorName);
        async Task result() => await authorRepository.RemoveAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(size - 1, _context.Authors.Count());
        Assert.Null(_context.Authors
            .SingleOrDefault(f => f.Name == authorName));
    }

    /// <summary>
    /// tests that removing an author that has never been in the database returns an error
    /// </summary>
    [Fact]
    public async void RemoveNonExistingAuthor()
    {
        //arrange
        string authorName = "someName";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act 
        async Task result() => await authorRepository.RemoveAuthor(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }

    /// <summary>
    /// tests that an author in the database can be found through their name
    /// </summary>
    [Fact]
    public async void FindAuthorByNameTest()
    {
        //arrange
        string authorName = "existingAuthor";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        AuthorDTO foundAuthor = await authorRepository.FindAuthorByName(authorName);
        //assert
        Assert.Equal("existingAuthor", foundAuthor.AuthorName);
    }

    /// <summary>
    /// tests that attempting to find an author that does not exist in the database through their name returns an error
    /// </summary>
    [Fact]
    public async void FindNonExistingAuthorByName()
    {
        //arrange
        string authorName = "someName";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act 
        async Task result() => await authorRepository.FindAuthorByName(authorName);

        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }


    /// <summary>
    /// checks that the DoesUserNameExists task gives a true on a existing user
    /// </summary>
    [Fact]
    public async void FindThatUserExistByName()
    {
        //Arrange
        string authorName = "existingAuthor";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        bool result = await authorRepository.DoesUserNameExists(authorName);

        //Assert
        Assert.True(result);
    }

    /// <summary>
    /// checks that the DoesUserNameExists task gives a false on a non existing user
    /// </summary>
    [Fact]
    public async void FindThatUserDoesNotExistByName()
    {
        //Arrange
        string authorName = "NonExistingAuthor";
        AuthorRepository authorRepository = new AuthorRepository(_context);

        //act
        bool result = await authorRepository.DoesUserNameExists(authorName);

        //Assert
        Assert.False(result);
    }
}