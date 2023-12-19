using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace test.UITests;

public class UiTests : PageTest, IClassFixture<TestWebApplicationFactory>
{
    private readonly string _serverAddress;

    public UiTests(TestWebApplicationFactory fixture)
    {
        _serverAddress = fixture.ServerAddress;
    }
    
    
    [Fact]
    public async Task Navigate_to_counter_ensure_current_counter_increases_on_click()
    {
        //Arrange
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        //Act
        await page.GotoAsync(_serverAddress);
        await page.ClickAsync("[class='nav-link']");
        await page.ClickAsync("[class='btn btn-primary']");

        //Assert
        await Expect(page.Locator("[role='status']")).ToHaveTextAsync("Current count: 1");
    }
}