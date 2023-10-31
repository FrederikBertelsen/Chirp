using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class CheepRepositoryTest {

    public CheepRepository setupDatabase() {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        using var context = new ChirpDBContext(builder.Options);
        return new CheepRepository(context);
    }

    public void testGetPageOfCheeps() {
        
    }
}