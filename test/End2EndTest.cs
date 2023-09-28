using System.Diagnostics;
using SimpleDB;
using Xunit.Abstractions;

namespace test;

public class End2EndTest
{
    private const string PathToTestCsvFile = CSVDatabase<Cheep>.CsvFilePath;
    private IDatabase<Cheep> testDatabase = CSVDatabase<Cheep>.Instance;

    private readonly ITestOutputHelper _testOutputHelper;
    public End2EndTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Sets up the csv database with 12 cheeps of test data.
    /// </summary>
    private void SetupTestCsvDatabase()
    {
        // delete test file if it already exists
        if (File.Exists(CSVDatabase<Cheep>.CsvFilePath)) File.Delete(PathToTestCsvFile);

        for (int i = 0; i < 12; i++)
            testDatabase.Store(new Cheep(1690891760, "testAuthor" + i, "testMessage" + i));
    }

    /// <summary>
    /// execute chirp application in a process with arguments.
    /// </summary>
    /// <param name="arguments">The arguments you want to execute Chirp with</param>
    /// <returns>a String containing the console output from Chirp</returns>
    private string ExecuteChirpInProcess(string arguments)
    {
        string output;
        using (var process = new Process())
        {
            process.StartInfo.FileName = "Chirp";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "./";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        return output;
    }

    [Fact]
    public void Chirp_ReadArgument_OutputsCorrectly()
    {
        // Arrange
        SetupTestCsvDatabase();

        // Act
        string output = ExecuteChirpInProcess("read");
        string firstCheep = output.Split("\n")[0];

        // Assert
        Assert.StartsWith("testAuthor0", firstCheep);
        Assert.EndsWith("testMessage0", firstCheep);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(11)]
    public void Chirp_ReadQuantity_CorrectAmount(int quantity)
    {
        // Arrange
        SetupTestCsvDatabase();

        // Act
        string output = ExecuteChirpInProcess("read " + quantity);
        string[] lines = output.Split("\n");

        // Assert
        // -1 just because
        Assert.Equal(quantity, lines.Length - 1);
    }
    
    [Theory]
    [InlineData("read -3")]
    [InlineData("read NotInt")]
    [InlineData("cheep 1")]
    [InlineData("cheep aadwfawdaw d waawd dw ad aw")]
    [InlineData("iDontKnowWhatImDoing")]
    public void Chirp_WrongArguments_PrintUsageHelpMessage(string arguments)
    {
        // Arrange
        SetupTestCsvDatabase();
        
        // Act
        string output = ExecuteChirpInProcess(arguments);
        string[] lines = output.Split("\n");
        
        // Assert
        Assert.StartsWith("Usage:",lines[0]);
    }
    
    [Fact]
    public void Chirp_StoreCheepReadCheep()
    {
        // Arrange
        // delete test file if it already exists
        if (File.Exists(CSVDatabase<Cheep>.CsvFilePath)) File.Delete(PathToTestCsvFile);
        
        // Act
        // write
        ExecuteChirpInProcess("cheep \"Hello ITU!\"");
        // read
        string output = ExecuteChirpInProcess("read");
        
        string[] lines = output.Split("\n");
        string lastLine = lines[lines.Length-2];
        
        // Assert
        Assert.EndsWith("Hello ITU!",lastLine);
    }
    
}