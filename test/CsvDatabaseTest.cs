using SimpleDB;

namespace test;

public class CsvDatabaseTest
{
    private const string PathToTestCsvFile = "../../../testdata/chirp_cli_test_db.csv";
    private IDatabase<Cheep> testDatabase = CSVDatabase<Cheep>.Instance;

    private void SetupTestCsvDatabase()
    {
        testDatabase = CSVDatabase<Cheep>.Instance;
        testDatabase.SetPath(PathToTestCsvFile);
    }

    /// <summary>
    /// Here we check if the database returns the correct amount of Cheeps from read().
    /// </summary>
    /// <param name="quantity">The quantity of Cheeps we want from the database</param>
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(11)]
    public void CsvDatabase_ReadQuantity_CorrectAmount(int? quantity)
    {
        // arrange
        SetupTestCsvDatabase();
        
        // act
        IEnumerable<Cheep> cheeps = testDatabase.Read(quantity);
        int readCount = cheeps.Count();
        
        // assert
        if (quantity == null)
            // null currently returns all the Cheeps in the .csv file.
            // Is this a bug or a feature
            Assert.Equal(0, readCount);
        else
            Assert.Equal(quantity, readCount);
    }

    [Fact]
    public void CsvDatabase_NegativeReadQuantity_ArgumentException()
    {
        // arrange
        SetupTestCsvDatabase();
        int quantity = -1;

        
        // act & assert
        Assert.Throws<ArgumentException>(() => testDatabase.Read(quantity));
    }
}