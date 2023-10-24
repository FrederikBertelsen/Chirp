namespace test;

public class UtilityTest
{
    // /// <summary>
    // /// Tests if the unixTile to DateTime conversion function throws an ArgumentException,
    // /// when the UnixTimeStamp is negative
    // /// </summary>
    // [Fact]
    // public void Utility_UnixTimeStampToDateTime_NegativeUnixTime_ArgumentException()
    // {
    //     // arrange
    //     Double timeStamp = -1695034267;
    //     //  act & assert
    //     Assert.Throws<ArgumentException>(() => Utility.UnixTimeStampToDateTime(timeStamp));
    // }
    //
    // /// <summary>
    // /// checks that the conversion of the function is correct.
    // /// </summary>
    // [Theory]
    // [InlineData(1695126565, 2023, 9, 19, 14, 29, 25)]
    // [InlineData(977691600, 2000, 12, 24, 22, 0, 0)]
    // [InlineData(3133650000, 2069, 4, 20, 4, 20, 0)]
    // public void Utility_UnixTimeStampToDateTime(double timeStamp, int year, int month, int day, int hour, int minute,
    //     int second)
    // {
    //     // arrange
    //     DateTime datetime = new DateTime(year, month, day, hour, minute, second);
    //     // act
    //     DateTime dateTimeFromUnixTimeStamp = Utility.UnixTimeStampToDateTime(timeStamp);
    //
    //     // assert
    //     Assert.Equal(datetime, dateTimeFromUnixTimeStamp);
    // }
}