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
    [InlineData("existingAuthor", "Hello", "2023-08-01 13:14:37")]
    [InlineData("existingAuthor", "this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. this string is exact 160 chars. ", "2023-08-01 13:14:37")]
    [InlineData("existingAuthor", "1", "2023-08-01 13:14:37")]
    public async void CreateCheep(string author, string text, string dateTime) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        Assert.Equal(cheepcount+1, context.Cheeps.Count());
    }
    
    [Theory]
    [InlineData("existingAuthor",null, null)]
    [InlineData("existingAuthor",null, "2023-08-01 13:14:37")]
    [InlineData("existingAuthor","Hello", null)]

    public async void CreateCheepWithNullValues(string author, string text, string dateTime) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }
    [Theory]
    [InlineData("nonExisitngAuthor","Hello", "2023-08-01 13:14:37")]

    public async void CreateCheepAuthorDoesNotExist(string author, string text, string dateTime) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }
    [Theory]
    [InlineData(null,"Hello", "2023-08-01 13:14:37")]
    public async void CreateCheepAuthorNull(string author, string text, string dateTime) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }
    /*[Theory]
    [InlineData("existingAuthor", "", "2023-08-01 13:14:37")]
    [InlineData("existingAuthor", "this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars..", "2023-08-01 13:14:37")]
    public async void CreateCheepLimits(string author, string text, string dateTime) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }*/
    public void testRemoveCheep() {

    }

    public void testRemoveCheepDoesNotExist() {

    }

    public void testGetCheepsByAuthor() {

    }

    public void testGetCheepsByAuthorDoesNotExist() {

    }
}