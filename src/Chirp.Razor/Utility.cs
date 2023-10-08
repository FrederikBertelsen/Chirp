using System.Globalization;
using Chirp.Razor;
using Chirp.Razor.Pages;

public class Utility
{
    /// <summary>
    ///     Creates a datetime object from a miliseconds timestamp.
    ///     Code is taken from
    ///     https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
    /// </summary>
    /// <param name="unixTimeStamp">timestamp in miliseconds from jan 1st 1970 00:00:00</param>
    /// <returns>Datetime object</returns>
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        if (unixTimeStamp < 0)
            throw new ArgumentException("unixTimeStamp can not be negative");

        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    /// <summary>
    ///     Creates a string object from a miliseconds timestamp.
    /// </summary>
    /// <param name="unixTimeStamp">timestamp in miliseconds from jan 1st 1970 00:00:00</param>
    /// <returns>a string containing the date</returns>
    public static string UnixTimeStampToFormatString(double unixTimeStamp)
    {
        var dateTime = UnixTimeStampToDateTime(unixTimeStamp);
        return dateTime.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Converts a list of Cheep objects from the database to a list of Cheep Records.
    /// </summary>
    /// <param name="dbCheeps">A list of cheep objects</param>
    /// <returns>A list of Cheep Records</returns>
    public static List<CheepViewModel> DbCheepsToRecordCheeps(List<Cheep> dbCheeps)
    {
        if (dbCheeps[0].Author == null)
            throw new ArgumentException(
                "Cheep.Author is null! Remember to include the author in the Cheeps that need to be displayed.");

        return (
            from cheep in dbCheeps
            select new CheepViewModel(cheep.Author.Name, cheep.Text,
                cheep.TimeStamp.ToString(CultureInfo.InvariantCulture))
        ).ToList();
    }
}