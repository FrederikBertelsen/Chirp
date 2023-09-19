namespace test;

public class CheepTest
{
    /// <summary>
    /// tests that the cheep constructor saves the arguments correctly
    /// </summary>
    [Fact]
    public void CheepConstruction()
    {
        // arrange
        double timestamp = 1695034276;
        string author = "Henrik";
        string message = "boomba";

        // act
        Cheep cheepTest = new Cheep(timestamp, author, message);

        // assert
        Assert.Equal(1695034276, cheepTest.Timestamp);
        Assert.Equal("Henrik", cheepTest.Author);
        Assert.Equal("boomba", cheepTest.Message);
    }

    /// <summary>
    /// test if the Author argument in the Cheep constructor can be an empty string
    /// it should throw an ArgumentException error 
    /// </summary>
    [Fact]
    public void CheepConstruction_EmptyAuthor_Exception()
    {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => new Cheep(1695034276, "", "Hello"));
    }

    /// <summary>
    /// test if the Message argument in the Cheep constructor can be an empty string
    /// it should throw an ArgumentException error 
    /// </summary>
    [Fact]
    public void CheepConstruction_EmptyMessage_Exception()
    {
        //arrange & act & assert
        Assert.Throws<ArgumentException>(() => new Cheep(1695034267, "Hansen", ""));
    }

    /// <summary>
    /// test if the TimeStamp argument in the Cheep constructor can be negative
    /// it should throw an ArgumentException error 
    /// </summary>
    [Fact]
    public void CheepConstruction_NegativeTimestamp_Exception()
    {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => new Cheep(-1695034267, "Hansen", "Nice"));
    }


    /// <summary>
    /// test if the String arguments in the Cheep constructor can be null 
    /// it should throw an ArgumentNullException error 
    /// </summary>
    [Theory]
    [InlineData(123456789, null, "hopla")]
    [InlineData(123456789, "Peter", null)]
    [InlineData(123456789, null, null)]
    public void CheepConstruction_NullArgument_Exception(double timeStamp, string author, string message)
    {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => new Cheep(timeStamp, author, message));
    }


    /// <summary>
    /// test if the Cheep ToString() function formats the data correctly
    /// </summary>
    [Fact]
    public void cheep_ToString()
    {
        // arrange
        double timestamp = 1695034276;
        string author = "Henrik";
        string message = "boomba";

        // act
        Cheep cheepTest = new Cheep(timestamp, author, message);

        // assert
        Assert.Equal("Henrik @ 18-09-2023 12:51:16: boomba", cheepTest.ToString());
    }
}