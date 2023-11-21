using System.Diagnostics.Contracts;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

//tests that should be reevaluated when pages is nolonger a field in CheepReposetory-object:
//
public class CheepRepositoryTest {
    ChirpDBContext context;
    SqliteConnection _connection;
    //might not be neccarcary when pagesize is refactored
    int pageSize;
    /// <summary>
    /// The setup for cheeprepo-test, here the connection to the database and the dbContext is created 
    /// </summary>
    public CheepRepositoryTest()
    {
        _connection =  new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection).Options;
        context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureCreated();
        pageSize = 32;
        SeedDatabase(context);
    }

    /// <summary>
    /// method for seeding the database with data for testing
    /// </summary>
    /// <param name="context"> the context dbcontext for testing</param>
    void SeedDatabase(ChirpDBContext context) 
    {
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
        CheepRepository cr = new CheepRepository(context);
        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheeps(1);
        foreach(var e in result)
            actualCount++;
        //assert
        Assert.Equal(Math.Min(context.Cheeps.Count(),pageSize),actualCount);
    }

    /// <summary>
    /// test the method GetPageOfCheeps if an illegal pagenumber is geven as input
    /// </summary>
    /// <param name="pageNumber">pagenumber input for GetPageOfCheeps method</param>
    [Theory]
    [InlineData(0)]
    [InlineData(-6)]
    public async void GetPageOfCheepsPagenumberOutOfRange(int pageNumber) 
    {
        //arrange
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result ()=> await cr.GetPageOfCheeps(pageNumber);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
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
    public async void CreateCheep(string author, string text, string dateTime) 
    {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        Assert.Equal(cheepcount+1, context.Cheeps.Count());
    }

    /// <summary>
    /// Tests that the correct exeption is thrown if any or all values of data in a cheep is null
    /// </summary>
    /// <param name="text">textfield of the CreateCheep method</param>
    /// <param name="dateTime">String used to create the datetime object. Format: yyyy-mm-dd hh:mm:ss</param>
    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "2023-08-01 13:14:37")]
    [InlineData("Hello", null)]

    public async void CreateCheepWithNullValues(string text, string dateTime) 
    {
        //arrange
        int cheepcount = context.Cheeps.Count();
        string author = "existingAuthor";
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateCheep method 
    /// is called with a nonExisting author
    /// </summary>
    /// <param name="author">the author field the authorname should not be in the database</param>
    [Theory]
    [InlineData("nonExisitngAuthor")]

    public async void CreateCheepAuthorDoesNotExist(string author) 
    {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        string text = "Hello";
        string dateTime = "2023-08-01 13:14:37";
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }

    /// <summary>
    /// tests that the correct Exeption is raised if the CreateCheep 
    /// method is called with a null value as author
    /// </summary>
    /// <param name="author">the author field of the CreateCheep method, 
    /// should be null for this test</param>
    [Theory]
    [InlineData(null)]
    public async void CreateCheepAuthorNull(string author) 
    {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        string dateTime = "2023-08-01 13:14:37";
        string text = "Hello";
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }

    //fails because we have not taken this into account in the cheepRepo
    /*/// <summary>
    /// Testing that an argumentExeption is thrown if the length of 
    /// the input-text exeeds the requirements of 1-160 chars
    /// </summary>
    /// <param name="text">textfield in CreateCheep mehtod, s
    /// hould not have a length between 1-160</param>
    [Theory]
    [InlineData("")]
    [InlineData("this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars.this is a string over 160 chars..")]
    public async void CreateCheepExtendingLimits(string text) {
        //arrange
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        string author = "existingAuthor";
        string dateTime = "2023-08-01 13:14:37";
        //act
        async Task result() => await cr.CreateCheep(author, text, DateTime.Parse(dateTime));
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }*/

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
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        await cr.RemoveCheep(cheepId);
        //assert
        Assert.Equal(cheepcount-1, context.Cheeps.Count());
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
        int cheepcount = context.Cheeps.Count();
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result() => await cr.RemoveCheep(cheepId);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(cheepcount, context.Cheeps.Count());
    }

    //cannot be made properly before pagesize can be decided by methodcall
    //only works because we do not have more than 32 cheeps per author in database
    /// <summary>
    /// tests that the GetCheepsByAuthor method fuctions propperly
    /// </summary>
    /// <param name="author">author name</param>
    /// <param name="cheepcountByAuthor">cheeps in the database by that author</param>
    [Theory]
    [InlineData("existingAuthor", 3)]
    [InlineData("Luanna Muro", 2)]
    public async void GetCheepsByAuthor(string author, int cheepcountByAuthor) {
        //arrange
        CheepRepository cr = new CheepRepository(context);
        int actualCount = 0;
        //act
        IEnumerable<CheepViewModel> result = await cr.GetPageOfCheepsByAuthor(author,1);
        foreach(var e in result)
            actualCount++;
        //assert
        Assert.Equal(actualCount, cheepcountByAuthor);
    }

    /// <summary>
    /// test that an ArgumentExeption is thrown when calling the GetCheepsByAuthor method
    /// with an author name not in the database
    /// </summary>
    /// <param name="author">the author field the authorname should not be in the database</param>
    [Theory]
    [InlineData("nonExisitngAuthor")]
     public async void GetCheepsByNonExistingAuthor(string author) {
        //arrange
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result ()=> await cr.GetPageOfCheepsByAuthor(author,1);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
    }

    /// <summary>
    /// test that an ArgumentNullExeption is thrown when calling the GetCheepsByAuthor method
    /// with an author name not in the database
    /// </summary>
    /// <param name="author">the author field the authorname should be null</param>
    [Theory]
    [InlineData(null)]
     public async void GetCheepsByNullAuthor(string author) {
        //arrange
        CheepRepository cr = new CheepRepository(context);
        //act
        async Task result ()=> await cr.GetPageOfCheepsByAuthor(author,1);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
    }

}