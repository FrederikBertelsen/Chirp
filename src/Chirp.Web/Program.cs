using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Configuration.AddUserSecrets("id");
if (builder.Configuration.GetConnectionString("Chirp") != null) {
    builder.Services.AddDbContext<ChirpDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Chirp")));
}
else {
    builder.Configuration.AddEnvironmentVariables();
    var AZURE_DB_SERVER = builder.Configuration["AZURE_DB_SERVER"];
    var AZURE_DB_NAME = builder.Configuration["AZURE_DB_NAME"];
    var AZURE_DB_USER = builder.Configuration["AZURE_DB_USER"];
    var AZURE_DB_PASSWORD = builder.Configuration["AZURE_DB_PASSWORD"];
    var connectionString = $"Server={AZURE_DB_SERVER};Initial Catalog={AZURE_DB_NAME};Persist Security Info=False;User ID={AZURE_DB_USER};Password={AZURE_DB_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    builder.Services.AddDbContext<ChirpDBContext>(options =>
        options.UseSqlServer(connectionString));
}
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();

// add authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


WebApplication app = builder.Build();


using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ChirpDBContext context = services.GetRequiredService<ChirpDBContext>();
    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// make the Chirp.Web Program class public so test project can access it
public partial class Program { }