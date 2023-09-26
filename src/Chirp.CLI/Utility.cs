public class Utility {
    /// <summary>
    /// Creates a datetime object from a miliseconds timestamp.
    /// Code is taken from https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa 
    /// </summary>
    /// <param name="unixTimeStamp">timestamp in miliseconds from jan 1st 1970 00:00:00</param>
    /// <returns>Datetime object</returns>
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp )
    {
        if (unixTimeStamp < 0)
            throw new ArgumentException("unixTimeStamp can not be negative");  
        
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}