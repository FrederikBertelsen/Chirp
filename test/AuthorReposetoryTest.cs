using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace test;

public class AuthorRepositoryTest{


    public static AuthorRepository BasicDatabaseInitializer(){
    
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
    /// A test of the different input options for name and email in the method AuthorRepository.CreateAuthor
    /// this test tests for null in both spaces, normal inputs, inputs with special charactors, and email in the wrong format
    /// </summary>
    /// <param name="authorName">the authorName atrubute in CreateAuthor method</param>
    /// <param name="authorEmail">the authorEmail atrubute in CreateAuthor method</param>
    [Theory]
    //basic format
    [InlineData("someName","someEmail@mail.com")]
    //name with space in 
    [InlineData("some Name","someEmail@mail.com")]
    //name with special charectors
    [InlineData("some_Name?!\\","someEmail@mail.com")]
    //name as null
    [InlineData(null,"someEmail@mail.com")]
    //missing line for too long name (how many chactors are max)
    //[InlineData("someTooLongName","someEmail@mail.com")]

    //email in wrong fromat
    [InlineData("someName","notAnEmail")]
    [InlineData("someName","notAnEmail.something")]
    [InlineData("someName","notAnEmail@something")]
    //email with special charactors
    [InlineData("someName","some.?\\Email@mail.com")]
    //mail as unll
    [InlineData("someName",null)]
    public void CreateAuthorTest(string authorName, string authorEmail){
        
    }
    [Fact]
    public async void CreateExistingAuthorTest(){
        string existingAuthor = "existingAuthor";
        string existingEmail = "existingEmail@mail.com";
        //some code to add author with name "existingAuthor"
        AuthorRepository authorRepository = BasicDatabaseInitializer();
        Action action = async () => authorRepository.CreateAuthor(existingAuthor, existingEmail);
        await Assert.ThrowsAsync<Exception>(action);
    }
}