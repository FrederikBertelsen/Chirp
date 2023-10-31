using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.Abstractions;
using Xunit;

namespace test;

public class IntegrationTesting : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public IntegrationTesting(WebApplicationFactory<Program> factory)
    {
        // Arrange
        this.factory = factory;
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    public async void CanSeePrivateTimeline(string author)
    {
        // Act
        HttpResponseMessage response = await client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        // Arrange
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Jacqualine Gilcoine")]
    public async void ThereAre32CheepsInPage(string page)
    {
        // Act
        HttpResponseMessage response = await client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();

        string contentOneLine = Regex.Replace(content, "\n", "");
        GroupCollection cheepListMatches =
            Regex.Match(contentOneLine, "<ul id=\"messagelist\" class=\"cheeps\">.*<\\/ul>").Groups;
        string cheepListStr = cheepListMatches[0].ToString();

        int listElementCount = Regex.Matches(cheepListStr, "<li>").Count;

        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Single(cheepListMatches);
        Assert.Equal(32, listElementCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Jacqualine Gilcoine")]
    public async void PaginationTest(string author)
    {
        // Act
        // get HTML
        HttpResponseMessage response = await client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        string page = await response.Content.ReadAsStringAsync();
        
        response = await client.GetAsync($"/{author}?page=1");
        response.EnsureSuccessStatusCode();
        string page1 = await response.Content.ReadAsStringAsync();
        
        response = await client.GetAsync($"/{author}?page=2");
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
}