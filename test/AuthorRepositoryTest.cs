using Xunit;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


public class AuthorRepositoryTest {

    public AuthorRepository setupDatabase() {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        using var context = new ChirpDBContext(builder.Options);

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
        return new AuthorRepository(context);
    }
    /// <summary>
    /// Test that the construcotr for AuthorReposetory executes correctly
    /// </summary>
    [Fact]
    public void AuthorRepositoryConstructorTest(){

    }

    /// <summary>
    /// Test that the construcotr for AuthorReposetory with null input fails
    /// </summary>
    [Fact]
    public void AuthorRepositoryConstructorWithNullInputTest(){
        //should fail/throw exception
    }

    /// <summary>
    /// tests that different ways of creating an author executes correctly 
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="authorEmail"></param>
    [Theory]
    //basic format
    [InlineData("someName","someEmail@mail.com")]
    //name with space in 
    [InlineData("some Name","someEmail@mail.com")]
    //name with special charectors
    [InlineData("some_Name?!\\","someEmail@mail.com")]
    //email with special charactors
    [InlineData("someName","some.?\\Email@mail.com")]
    public void CreateNonexistingAuthor(string authorName, string authorEmail){
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="authorName"></param>
    /// <param name="authorEmail"></param>
    [Theory]
    [InlineData("existingAuthor","existingEmail@mail.com")]
    [InlineData("someName","existingEmail@mail.com")]
    [InlineData("someName","someEmail@mail.com")]
    public void CreateAuthorsWithExistingData(string authorName, string authorEmail){
        //should throw exception
    }
    [Theory]
    //both as null
    [InlineData(null,null)]
    //name as null
    [InlineData(null,"someEmail@mail.com")]
    //mail as unll
    [InlineData("someName",null)]
    public void CreateAuthorNullValue(string authorName, string authorEmail){

    }
    [Theory]
    //basic format
    [InlineData("someName","someEmail@mail.com")]
    public void CreateAuthorTwiceTest(string authorName, string authorEmail){

    }

    [Theory]
    //Existing author values
    [InlineData("existingAuthor", "existingEmail@mail.com")]
    public void RemoveExistingAuthorTest(string authorName, string authorEmail){
        
    }
    [Theory]
    //both as null
    [InlineData(null,null)]
    public void RemoveAuthorNullValue(string authorName, string authorEmail){

    }
    [Theory]
    //both as null
    [InlineData("existingAuthor", "existingEmail@mail.com")]
    public void RemoveAuthorTwice(string authorName, string authorEmail){

    }
    [Theory]
    //basic format
    [InlineData("someName","someEmail@mail.com")]
    public void RemoveNonexistingAuthor(string authorName, string authorEmail){

    }
    [Theory]
    //Existing author values
    [InlineData("existingAuthor")]
    public void FindAuthorByNameTest(string authorName){

    }
    [Theory]
    //basic format
    [InlineData("someName")]
    public void FindNonexistingAuthorByName(string authorName){

    }
    [Theory]
    //Existing author values
    [InlineData("existingEmail@mail.com")]
    public void FindAuthorByEmailTest(string authorEmail){

    }
    [Theory]
    //basic format
    [InlineData("someEmail@mail.com")]
    public void FindNonexistingAuthorEmail(string authorEmail){

    }
}
