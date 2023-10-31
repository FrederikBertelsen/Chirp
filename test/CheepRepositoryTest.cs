using System.Diagnostics.Contracts;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class CheepRepositoryTest {

    static List<Author> Authors;
    static List<Cheep> Cheeps;
    ChirpDBContext context;
    SqliteConnection _connection;
    public CheepRepositoryTest()
    {
        _connection =  new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection).Options;
        context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureCreated();
        SeedDatabase(context);
    }

    public void SeedDatabase(ChirpDBContext context) {
        var a1 = new Author() { AuthorId = 1, Name = "existingAuthor", Email = "existingEmail@mail.com", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Luanna Muro", Email = "Luanna-Muro@ku.dk", Cheeps = new List<Cheep>() };
        
        var authors = new List<Author>() { a1, a2};

        var c1 = new Cheep() { CheepId = 1, Author = a1, Text = "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
        var c2 = new Cheep() { CheepId = 2, Author = a1, Text = "And then, as he listened to all that''s left o'' twenty-one people.", TimeStamp = DateTime.Parse("2023-08-01 13:15:21") };
        var c3 = new Cheep() { CheepId = 3, Author = a2, Text = "In various enchanted attitudes, like the Sperm Whale.", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };
        var c4 = new Cheep() { CheepId = 4, Author = a1, Text = "Unless we succeed in establishing ourselves in some monomaniac way whatever significance might lurk in them.", TimeStamp = DateTime.Parse("2023-08-01 13:14:34") };
        var c5 = new Cheep() { CheepId = 5, Author = a2, Text = "At last we came back!", TimeStamp = DateTime.Parse("2023-08-01 13:14:35") };
        
        var cheeps = new List<Cheep>(){c1,c2,c3,c4,c5};

        a1.Cheeps = new List<Cheep>() { c1, c2,c4 };
        a2.Cheeps = new List<Cheep>() { c3,c5 };
        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        Cheeps = cheeps;
        Authors = authors;
        context.SaveChanges();
    }

    [Fact]
    public void testGetPageOfCheeps() {
        Assert.True(true);
    }

/// <summary>
/// Test of successfully creating a cheep
/// </summary>
/// <param name="authorIndex">The index of the author in the list of authors created during database setup</param>
/// <param name="text">Text of the created cheep</param>
/// <param name="dateTime">String used to create the datetime object. Format: yyyy-mm-dd hh:mm:ss</param>
    [Theory]
    [InlineData(1, "Hello", "2023-08-01 13:14:37")]

    public async void testCreateCheep(int authorIndex, string text, string dateTime) {
        Assert.Equal(5, context.Cheeps.Count());
        CheepRepository cr = new CheepRepository(context);
        await cr.CreateCheep("existingAuthor", text, DateTime.Parse(dateTime));
        Assert.Equal(6, context.Cheeps.Count());

    }

    public void testCreateCheepLimits(Author author, string text, DateTime dateTime) {

    }

    public void testCreateCheepNullValues(Author author, string text, DateTime dateTime) {

    }

    public void testCreateCheepAuthorDoesNotExist(Author author) {

    }

    public void testRemoveCheep() {

    }

    public void testRemoveCheepDoesNotExist() {

    }

    public void testGetCheepsByAuthor() {

    }

    public void testGetCheepsByAuthorDoesNotExist() {

    }
}