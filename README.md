Playwright + AXE Accessibility Testing Project

Set up for running automated tests with Playwright and AXE for accessibility checks. Language is .NET C#.

Libraries Used

using Deque.AxeCore.Commons;

using Deque.AxeCore.Playwright;

using Microsoft.Playwright;

API document reference

https://www.nuget.org/packages/Deque.AxeCore.Playwright

Quick tip

add { Timeout = 90000 } to
_page.GotoAsync(URL, new PageGotoOptions { Timeout = 90000 });  
to overwrite the default timeout of 30000

Run Tests

in Test Explorer of VS Code and Visual Studio Community
