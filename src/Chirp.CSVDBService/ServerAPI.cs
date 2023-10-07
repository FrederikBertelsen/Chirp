using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapGet("/", () => "Hello Worlds!");
app.MapGet("/cheeps/{limit}", (int limit) => CSVDatabase<Cheep>.Instance.Read(limit));
app.MapPost("/cheep", (Cheep cheep) => {
    CSVDatabase<Cheep>.Instance.Store(cheep);
    return Results.Created($"/cheeps", cheep);
    });


app.Run();