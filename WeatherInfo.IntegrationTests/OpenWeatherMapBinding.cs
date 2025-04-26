using Allure.Net.Commons;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;
using Reqnroll;
using Shouldly;

namespace OpenWeatherMap.E2ETests
{
    [Binding]
    public class OpenWeatherMapBindings : IAsyncLifetime
    {
        private readonly ScenarioContext _scenarioContext;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private BrowserNewContextOptions _contextOptions;

        public OpenWeatherMapBindings(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync()
        {
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AllureLifecycle.Instance.CleanupResultDirectory();
            Console.WriteLine($"Starting test run at {DateTime.Now}");
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
            });

            string videoDir = "videos";
            Directory.CreateDirectory(videoDir);

            _contextOptions = new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                RecordVideoDir = videoDir,
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            };

            var context = await _browser.NewContextAsync(_contextOptions);
            _page = await context.NewPageAsync();
            _scenarioContext["Page"] = _page;
        }

        private async Task ProcessVideoWithRetry(IVideo video, string scenarioName, bool hasFailed)
        {
            string videoPath = null;
            bool success = false;
            int maxAttempts = 3;

            try
            {
                videoPath = await video.PathAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting video path: {ex.Message}");
                return;
            }

            if (videoPath == null)
            {
                Console.WriteLine("Video path is null, cannot process video");
                return;
            }

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempt {attempt}: Checking video file at {videoPath}");

                    if (!File.Exists(videoPath))
                    {
                        Console.WriteLine($"Video file not found at {videoPath}, waiting...");
                        await Task.Delay(4000 * attempt);
                        continue;
                    }

                    using (var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var fileLength = fileStream.Length;
                        Console.WriteLine($"Video file size: {fileLength} bytes");

                        if (fileLength < 1000)
                        {
                            Console.WriteLine("Video file is too small, may still be writing...");
                            await Task.Delay(3000 * attempt);
                            continue;
                        }
                    }

                    byte[] videoBytes = await File.ReadAllBytesAsync(videoPath);

                    string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    string videoStatus = hasFailed ? "failed" : "passed";
                    string permanentVideoPath = Path.Combine("videos", $"{videoStatus}-{scenarioName}-{timestamp}.webm");

                    File.Copy(videoPath, permanentVideoPath, true);
                    Console.WriteLine($"Copied video to {permanentVideoPath}");

                    if (hasFailed)
                    {
                        Console.WriteLine("Adding video attachment to Allure report");
                        AllureApi.AddAttachment($"Test Recording - {scenarioName}", "video/webm", videoBytes);
                        AllureLifecycle.Instance.StopStep();
                        Console.WriteLine("Video added to Allure report successfully");
                    }

                    success = true;
                    break;
                }
                catch (IOException ex) when (ex.Message.Contains("used by another process"))
                {
                    Console.WriteLine($"Video file is locked (attempt {attempt}): {ex.Message}");
                    await Task.Delay(5000 * attempt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing video (attempt {attempt}): {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            if (!success)
            {
                Console.WriteLine($"Failed to process video after {maxAttempts} attempts");
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            var page = _scenarioContext["Page"] as IPage;
            if (page != null)
            {
                bool hasFailed = _scenarioContext.TestError != null;
                IVideo video = null;

                try
                {
                    video = page.Video;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to get video reference: {ex.Message}");
                }

                var context = page.Context;
                await page.CloseAsync();
                await context.CloseAsync();

                if (video != null && hasFailed)
                {
                    await ProcessVideoWithRetry(video, _scenarioContext.ScenarioInfo.Title, hasFailed);
                }
            }

            if (_browser != null)
                await _browser.CloseAsync();

            if (_playwright != null)
                _playwright.Dispose();
        }

        [Given("I navigate to OpenWeatherMap homepage")]
        public async Task GivenINavigateToOpenWeatherMapHomepage()
        {
            var page = _scenarioContext["Page"] as IPage;

            if (page == null)
            {
                throw new InvalidOperationException("Page is not initialized in the scenario context.");
            }

            await page.GotoAsync("https://openweathermap.org/");
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 10000 });
            await page.WaitForSelectorAsync("input[placeholder='Search city']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });
        }

        [When("I search for {string} in the weather search")]
        public async Task WhenISearchForCityInWeatherSearch(string cityName)
        {
            var page = _scenarioContext["Page"] as IPage;

            await page.FillAsync("input[placeholder='Search city']", cityName);
            var searchButton = page.Locator("button[type='submit']").Filter(new() { HasText = "Search" });
            await searchButton.ClickAsync();
            _scenarioContext["SearchTerm"] = cityName;
        }

        [When("Click on first result")]
        public async Task WhenClickOnFirstResult()
        {
            var page = _scenarioContext["Page"] as IPage;

            await page.WaitForTimeoutAsync(500);
            await page.WaitForSelectorAsync(".search-dropdown-menu li:first-child", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await page.ClickAsync(".search-dropdown-menu li:first-child");
            await page.WaitForTimeoutAsync(500);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 10000 });
        }

        [Then("I should see weather details for {string}")]
        public async Task ThenIShouldSeeWeatherDetailsForCity(string cityName)
        {
            var page = _scenarioContext["Page"] as IPage;

            await page.WaitForSelectorAsync(".current-container", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 10000 });

            var cityElement = await page.QuerySelectorAsync(".current-container h2");
            var cityText = await cityElement.TextContentAsync();
            cityText.ShouldContain(cityName, (Case)StringComparison.OrdinalIgnoreCase);

            var temperatureElement = await page.QuerySelectorAsync(".current-temp");
            temperatureElement.ShouldNotBeNull();

            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/success-search-{cityName}-{DateTime.Now:yyyyMMdd-HHmmss}.png"
            });
            AllureApi.AddAttachment($"Weather Details for {cityName}", "image/png", screenshotBytes);
        }

        [Then("I should see no results found message")]
        public async Task ThenIShouldSeeNoResultsFoundMessage()
        {
            var page = _scenarioContext["Page"] as IPage;

            await page.WaitForSelectorAsync("div.sub.not-found.notFoundOpen", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            var notFoundElement = await page.QuerySelectorAsync("div.sub.not-found.notFoundOpen");
            var notFoundText = await notFoundElement.TextContentAsync();
            notFoundText.ShouldContain("not found", (Case)StringComparison.OrdinalIgnoreCase);

            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/no-results-search-{DateTime.Now:yyyyMMdd-HHmmss}.png"
            });
            AllureApi.AddAttachment("No results search", "image/png", screenshotBytes);
        }
    }
}
