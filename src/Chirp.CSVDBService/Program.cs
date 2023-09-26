using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

//Cheep cheep = new (134,"someone", "something");
app.MapGet("/", () => "Hello Worlds!");
app.MapGet("/cheeps/{limit}", (int limit) => CSVDatabase<Cheep>.Instance.Read(limit));
app.MapPost("/cheep", (Cheep cheep) => {
    CSVDatabase<Cheep>.Instance.Store(cheep);
    return Results.Created($"/cheeps", cheep);
    });




app.Run();

public record Cheep(string Author, string Message, long Timestamp);