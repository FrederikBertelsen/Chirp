class Program
{
    private static readonly string PathToCsvFile = "data/chirp_cli_db.csv";
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            return;
        if (args[0].ToLower() == "read")
        {
            var cheeps = ChirpDataBase.Read(PathToCsvFile);
            foreach (var cheep in cheeps)
            {
                Console.WriteLine(cheep.ToString());
            }
        }


        if (args[0].ToLower() == "cheep")
        {
            if (args[1] == null)
            {
                Console.WriteLine("Text cannot be emtpy!");
                return;
            }

            string userName = Environment.UserName;
            string text = args[1];
            DateTimeOffset timestamp = DateTime.Now;
            Cheep cheep = new Cheep(timestamp.ToUnixTimeSeconds(), userName, text);
            ChirpDataBase.Write(PathToCsvFile, cheep);
        }
    }
}