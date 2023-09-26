using SimpleDB;

namespace test;

public class CsvDatabaseTest
{
    private const string PathToTestCsvFile = "./testdata/chirp_cli_test_db.csv";
    private IDatabase<Cheep> testDatabase = CSVDatabase<Cheep>.Instance;

    private void SetupTestCsvDatabase()
    {
        bool csvFileAlreadyExists = File.Exists(PathToTestCsvFile);

        testDatabase = CSVDatabase<Cheep>.Instance;
        testDatabase.SetFilePath(PathToTestCsvFile);

        if (!csvFileAlreadyExists)
        {
            testDatabase.SetFilePath(PathToTestCsvFile);
            testDatabase.Store(new Cheep(1690891760, "testAuthor0", "testMessage0"));
            testDatabase.Store(new Cheep(1690978778, "testAuthor1", "testMessage1"));
            testDatabase.Store(new Cheep(1690981487, "testAuthor2", "testMessage2"));
            testDatabase.Store(new Cheep(1690981487, "testAuthor3", "testMessage3"));
            testDatabase.Store(new Cheep(1690978778, "testAuthor4", "testMessage4"));
            testDatabase.Store(new Cheep(1690891760, "testAuthor5", "testMessage5"));
            testDatabase.Store(new Cheep(1690981487, "testAuthor6", "testMessage6"));
            testDatabase.Store(new Cheep(1690978778, "testAuthor7", "testMessage7"));
            testDatabase.Store(new Cheep(1690891760, "testAuthor8", "testMessage8"));
            testDatabase.Store(new Cheep(1690981487, "testAuthor9", "testMessage9"));
            testDatabase.Store(new Cheep(1690978778, "testAuthor10", "testMessage10"));
            testDatabase.Store(new Cheep(1690891760, "testAuthor11", "testMessage11"));
        }
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
    [Fact]
    public void CsvDatabase_NegativeReadQuantity_ArgumentException()
    {
        // arrange
        SetupTestCsvDatabase();
        int quantity = -1;

        // act & assert
        Assert.Throws<ArgumentException>(() => testDatabase.Read(quantity));
    }

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