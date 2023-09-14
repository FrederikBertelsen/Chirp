using CsvHelper.Configuration;

namespace Chirp.CLI;

public class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(cheep => cheep.Author).Index(0).Name("Author");
        Map(cheep => cheep.Message).Index(1).Name("Message");
        Map(cheep => cheep.Timestamp).Index(2).Name("Timestamp");
    }
}