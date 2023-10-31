using Xunit;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class AuthorRepositoryTest {

    public AuthorRepository setupDatabase() {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        using var context = new ChirpDBContext(builder.Options);
        return new AuthorRepository(context);
    }

    public void CreateAnAuthor(){

    }
    public void CreateAuthorsWithSameData(){

    }

    public void CreateAuthorNullValue(){

    }

    public void RemoveAnAuthor(){

    }

    public void RemoveAuthorNullValue(){

    }

    public void RemoveAuthorTwice(){

    }

    public void RemoveNonexistingAuthor(){

    }

    public void FindAuthorByNameTest(){

    }

    public void FindNonexistingAuthorByName(){

    }

    public void FindAuthorByEmailTest(){

    }

    public void FindNonexistingAuthorEmail(){

    }

}
