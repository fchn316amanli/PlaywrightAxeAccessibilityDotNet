using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace PlaywrightAxeAccessibilityDotNet;

public class AccessibilityTests
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _browserContext;
    private IPage _page;
    
    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        _browserContext = await _browser.NewContextAsync();
        _page = await _browserContext.NewPageAsync();
    }

    [Test]
    public async Task Test1ScanEntirePage()
    {

        await _page.GotoAsync("https://www.becu.org/", new PageGotoOptions { Timeout = 90000 });

        AxeResult accessibilityScanResults = await _page.RunAxe();
        
        Assert.That(accessibilityScanResults.Violations, Is.Null.Or.Empty, 
                "Accessibility violations found: " + string.Join(", ", accessibilityScanResults.Violations.Select(v => v.Id)));

    }

    [Test]
    public async Task Test2ScanPartPageInclude()
    {
        await _page.GotoAsync("https://www.becu.org/", new PageGotoOptions { Timeout = 90000 });

        await _page.Locator(".dropdown-toggle:has-text('Planning & Investing')").ClickAsync();
        Thread.Sleep(3000);

        AxeRunContext runContext = new AxeRunContext()
        {
            Include = new List<AxeSelector>() { 
                new AxeSelector(".flex-col"), //5 elements
                new AxeSelector("a[href=\"https://www.becu.org/blog\"]"), //3 elements
            } 
        };

        AxeResult accessibilityScanResults = await _page.RunAxe(runContext);
        Thread.Sleep(3000);

        Assert.That(accessibilityScanResults.Violations, Is.Null.Or.Empty);
    }


    [Test]
    public async Task Test3ScanPartPageExclude()
    {
        await _page.GotoAsync("https://www.becu.org/", new PageGotoOptions { Timeout = 90000 });

        AxeRunContext runContext = new AxeRunContext()
        {
            Exclude = new List<AxeSelector>() {
                new AxeSelector("h4.heading2.m-0.px-3.py-4"),//7 elements
                new AxeSelector("div#skipfootercontent") // Exclude the entire footer
            }
        };
        
        AxeResult accessibilityScanResults = await _page.RunAxe(runContext);
        Thread.Sleep(3000);

        Assert.That(accessibilityScanResults.Violations, Is.Null.Or.Empty);
    }


    [Test]
    public async Task Test4ScanTags()
    {
        await _page.GotoAsync("https://www.becu.org/", new PageGotoOptions { Timeout = 90000 });

        AxeRunOptions options = new AxeRunOptions()
        {
            RunOnly = new RunOnlyOptions 
            { Type = "tag", Values = new List<string> 
                { "wcag2aa", "wcag2a", "wcag21a", "wcag21aa" } 
            },
        };

        AxeResult accessibilityScanResults = await _page.RunAxe(options);
        Thread.Sleep(3000);

        Assert.That(accessibilityScanResults.Violations, Is.Null.Or.Empty);
    }


    [Test]
    public async Task Test5ScanRulesDisabled()
    {
        await _page.GotoAsync("https://www.becu.org/", new PageGotoOptions { Timeout = 90000 });

        AxeRunOptions options = new AxeRunOptions()
        {
            Rules = new Dictionary<string, RuleOptions>()
            {
                { "heading-order", new RuleOptions() { Enabled = false } },
                { "region", new RuleOptions() { Enabled = false } }
            },
        };

        AxeResult accessibilityScanResults = await _page.RunAxe(options);
        Thread.Sleep(3000);

        Assert.That(accessibilityScanResults.Violations, Is.Null.Or.Empty);
    }


    [TearDown]
    public async Task Teardown()
    {
        // Close resources in reverse order of creation
        if (_page != null) await _page.CloseAsync();
        if (_browserContext != null) await _browserContext.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}
