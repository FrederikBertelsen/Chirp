using System.Diagnostics;
using SimpleDB;

namespace test;

public class End2EndTest
{
    private const string PathToTestCsvFile = "./testdata/chirp_cli_test_db.csv";
    private IDatabase<Cheep> testDatabase = CSVDatabase<Cheep>.Instance;
    
    /// <summary>
    /// Sets up the csv database with 12 cheeps of test data.
    /// </summary>
    private void SetupTestCsvDatabase()
    {
        testDatabase = CSVDatabase<Cheep>.Instance;

        // delete test file if it already exists
        if (File.Exists(PathToTestCsvFile)) File.Delete(PathToTestCsvFile);
        testDatabase.SetFilePath(PathToTestCsvFile);
        
        for (int i = 0; i < 12; i++)
        {
            testDatabase.Store(new Cheep(1690891760, "testAuthor" + i, "testMessage" + i));
        }
    }
    
    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        SetupTestCsvDatabase();
        
        // Act
        string output;
        using (var process = new Process())
        {
            process.StartInfo.FileName = "Chirp.dll";
            process.StartInfo.Arguments = "./src/Chirp.CLI/bin/Debug/net7.0/Chirp.dll read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string fstCheep = output.Split("\n")[0];
        
        // Assert
        Assert.StartsWith("testAuthor0", fstCheep);
        Assert.EndsWith("testMessage0", fstCheep);
    }
}