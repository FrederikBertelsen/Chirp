using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace test;

public class IntegrationTesting : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTesting(WebApplicationFactory<Program> factory)
    {
        // Arrange
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });
    }

    /// <summary>
    /// this test checks that we are on a chirp site and if we can access the public timeline
    /// by asserting that the html code on our frontpage/publictimeline contains the app title text Chirp!
    /// and the page title public timeline.
    /// </summary>
    [Fact]
    public async void CanSeePublicTimeline()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    /// <summary>
    ///  this test checks that we go to a users private timeline when we specify a user in the url 
    /// </summary>
    /// <param name="author"> the specified user</param>
    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void CanSeePrivateTimeline(string author)
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        // Arrange
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    /// <summary>
    /// here we cheek that there are exactly 32 cheeps in a full page of cheeps
    /// both on the public and on a private timeline
    /// </summary>
    /// <param name="page">the specified page(public or private)</param>
    [Theory]
    [InlineData("")]
    [InlineData("Jacqualine Gilcoine")]
    public async void ThereAre32CheepsInPage(string page)
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        string contentOneLine = Regex.Replace(content, "\n", "");
        GroupCollection cheepListMatches =
            Regex.Match(contentOneLine, "<ul id=\"messagelist\" class=\"cheeps\">.*<\\/ul>").Groups;
        string cheepListStr = cheepListMatches[0].ToString();

        int listElementCount = Regex.Matches(cheepListStr, "<li>").Count;

        response = await _client.GetAsync($"/{page}?page=2");
        response.EnsureSuccessStatusCode();
        string page2 = await response.Content.ReadAsStringAsync();

        string contentOneLinePage2 = Regex.Replace(content, "\n", "");
        GroupCollection cheepListMatchesPage2 =
            Regex.Match(contentOneLine, "<ul id=\"messagelist\" class=\"cheeps\">.*<\\/ul>").Groups;
        string cheepListStrPage2 = cheepListMatches[0].ToString();
        int listElementCountPage2 = Regex.Matches(cheepListStr, "<li>").Count;


        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Single(cheepListMatches);
        Assert.Equal(32, listElementCount);
        Assert.True(listElementCountPage2 <= 32);
    }

    /// <summary>
    /// Tests that page 1 is the same as not using ?page=X
    /// And that page 1 != page 2
    /// </summary>
    /// <param name="pageURL">The page the request will be made on</param>
    [Theory]
    [InlineData("")]
    [InlineData("Jacqualine Gilcoine")]
    public async void PaginationTest_MoreThanOnePage(string pageURL)
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"/{pageURL}");
        response.EnsureSuccessStatusCode();
        string page = await response.Content.ReadAsStringAsync();

        response = await _client.GetAsync($"/{pageURL}?page=1");
        response.EnsureSuccessStatusCode();
        string page1 = await response.Content.ReadAsStringAsync();

        response = await _client.GetAsync($"/{pageURL}?page=2");
        response.EnsureSuccessStatusCode();
        string page2 = await response.Content.ReadAsStringAsync();

        // remove new-lines, so the strings are easier to match with regex
        page = Regex.Replace(page, "\n", "");
        page1 = Regex.Replace(page1, "\n", "");
        page2 = Regex.Replace(page2, "\n", "");


        // get first cheeps of each page
        string firstCheepRegex =
            "id=\"messagelist\" class=\"cheeps\">\\s+<li>.+?<\\/strong>\\s*(.*?)\\s*<small>.*?<\\/li>";
        string[] firstCheeps = new string[3];
        firstCheeps[0] = Regex.Match(page, firstCheepRegex).Groups[1].ToString();
        firstCheeps[1] = Regex.Match(page1, firstCheepRegex).Groups[1].ToString();
        firstCheeps[2] = Regex.Match(page2, firstCheepRegex).Groups[1].ToString();

        // Assert
        Assert.Contains("Chirp!", page);
        Assert.Contains("Chirp!", page1);
        Assert.Contains("Chirp!", page2);

        Assert.Equal(firstCheeps[0], firstCheeps[1]);
        Assert.NotEqual(firstCheeps[1], firstCheeps[2]);
    }


    /// <summary>
    /// Tests that our PageNotFound page displays correctly
    /// </summary>
    /// <param name="pageURL">The page the request will be made on</param>
    [Theory]
    [InlineData("Jacqualine Gilcoine?page=999")]
    [InlineData("Non Existing Author")]
    public async void PaginationTest_PageNotFound(string pageURL)
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync($"/{pageURL}");
        response.EnsureSuccessStatusCode();
        string page = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp!", page);
        Assert.Contains("There are no cheeps here.", page);
    }
}