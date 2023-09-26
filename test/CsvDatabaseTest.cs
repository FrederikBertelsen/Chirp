using SimpleDB;

namespace test;

public class CsvDatabaseTest
{
    private const string PathToTestCsvFile = CsvDatabase<Cheep>.CsvFilePath;
    private IDatabase<Cheep> testDatabase = CsvDatabase<Cheep>.Instance;

    /// <summary>
    /// Sets up the csv database with 12 cheeps of test data.
    /// </summary>
    private void SetupTestCsvDatabase()
    {
        // delete test file if it already exists
        if (File.Exists(CsvDatabase<Cheep>.CsvFilePath)) File.Delete(PathToTestCsvFile);
        
        for (int i = 0; i < 12; i++) 
            testDatabase.Store(new Cheep(1690891760, "testAuthor" + i, "testMessage" + i));
    }

    /// <summary>
    /// Here we check if the database returns the correct amount of Cheeps from read().
    /// We used code from here to get line count of csv file: https://stackoverflow.com/questions/119559/determine-the-number-of-lines-within-a-text-file
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
        // -1 because of the .csv file format header
        int csvFileLineCount = File.ReadAllLines(PathToTestCsvFile).Length - 1;

        // act
        IEnumerable<Cheep> testCheeps = testDatabase.Read(quantity);
        int readCount = testCheeps.Count();

        // assert
        if (quantity == null)
            Assert.Equal(csvFileLineCount, readCount);

        else
            Assert.Equal(quantity, readCount);
    }

    /// <summary>
    /// here we test that inputing a negative number into read gives an ArgumentException
    /// </summary>
    // [Fact]
    // public void CsvDatabase_NegativeReadQuantity_ArgumentException()
    // {
    //     // arrange
    //     SetupTestCsvDatabase();
    //     int quantity = -1;
    //
    //     // act & assert
    //     Assert.Throws<ArgumentException>(() => testDatabase.Read(quantity));
    // }

    /// <summary>
    /// here we test that that cheeps stored via the store() function are correctly formatted
    /// and can be fetched via the read() function
    /// </summary>
    [Fact]
    public void CsvDatabase_StoreInCsvFile()
    {
        //arrange
        SetupTestCsvDatabase();
        double timestamp = 1695034276;
        string author = "Henrik";
        string message = "boomba";
        Cheep testCheep = new Cheep(timestamp, author, message);

        //act
        testDatabase.Store(testCheep);
        IEnumerable<Cheep> testCheeps = testDatabase.Read();
        Cheep storedCheep = testCheeps.Last();
        //assert

        Assert.Equal(testCheep.Timestamp, storedCheep.Timestamp);
        Assert.Equal(testCheep.Author, storedCheep.Author);
        Assert.Equal(testCheep.Message, storedCheep.Message);
    }
}