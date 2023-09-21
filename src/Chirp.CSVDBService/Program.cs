using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Cheep cheep = new (134,"someone", "something");
app.MapGet("/", () => "Hello Worlds!");
app.MapGet("/cheeps/", CSVDatabase<Cheep>.Instance.Read);
app.MapPost("/cheep/", (Cheep cheep) => CSVDatabase<Cheep>.Instance.Store);




app.Run();