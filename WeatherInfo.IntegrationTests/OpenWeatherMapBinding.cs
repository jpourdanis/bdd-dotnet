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
            // Ensure Allure directory exists and is clean
            AllureLifecycle.Instance.CleanupResultDirectory();

            // Log beginning of test run
            Console.WriteLine($"Starting test run at {DateTime.Now}");
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            // Create Playwright instance
            _playwright = await Playwright.CreateAsync();

            // Launch browser with visible UI for debugging
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,  // Set to true in CI environments
            });

            // Define recording directory and ensure it exists
            string videoDir = "videos";
            Directory.CreateDirectory(videoDir);

            // Configure browser context with video recording
            _contextOptions = new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = 1920,
                    Height = 1080
                },
                RecordVideoDir = videoDir,
                RecordVideoSize = new RecordVideoSize
                {
                    Width = 1280,
                    Height = 720
                }
            };

            // Create context and page
            var context = await _browser.NewContextAsync(_contextOptions);
            await context.Tracing.StartAsync(new TracingStartOptions { Screenshots = true, Snapshots = true, Sources = true });
            _page = await context.NewPageAsync();
            _scenarioContext["Page"] = _page;

        }
        // Helper method to process video with retry mechanism
        private async Task ProcessVideoWithRetry(IVideo video, string scenarioName, bool hasFailed)
        {
            string videoPath = null;
            bool success = false;
            int maxAttempts = 3;

            // Get video path
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

            // Wait and retry approach for file access
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempt {attempt}: Checking video file at {videoPath}");

                    // First make sure the file exists
                    if (!File.Exists(videoPath))
                    {
                        Console.WriteLine($"Video file not found at {videoPath}, waiting...");
                        await Task.Delay(4000 * attempt); // Increase delay with each attempt
                        continue;
                    }

                    // Check if file is accessible and not locked
                    using (var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // Get file size
                        var fileLength = fileStream.Length;
                        Console.WriteLine($"Video file size: {fileLength} bytes");

                        // If file is too small, it might still be writing
                        if (fileLength < 1000)
                        {
                            Console.WriteLine("Video file is too small, may still be writing...");
                            await Task.Delay(3000 * attempt);
                            continue;
                        }
                    }

                    // Read the bytes
                    byte[] videoBytes = await File.ReadAllBytesAsync(videoPath);

                    // Create a permanent video path with timestamp
                    string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    string videoStatus = hasFailed ? "failed" : "passed";
                    string permanentVideoPath = Path.Combine(
                        "videos",
                        $"{videoStatus}-{scenarioName}-{timestamp}.webm"
                    );

                    // Copy video to permanent location
                    File.Copy(videoPath, permanentVideoPath, true);
                    Console.WriteLine($"Copied video to {permanentVideoPath}");

                    // For failed scenarios, attach to Allure
                    if (hasFailed)
                    {
                        Console.WriteLine("Adding video attachment to Allure report");
                        // Use AllureApi to add attachment
                        AllureApi.AddAttachment(
                            $"Test Recording - {scenarioName}",
                            "video/webm",
                            videoBytes
                        );

                        // Explicitly flush step
                        AllureLifecycle.Instance.StopStep();
                        Console.WriteLine("Video added to Allure report successfully");
                    }

                    success = true;
                    break;
                }
                catch (IOException ex) when (ex.Message.Contains("used by another process"))
                {
                    // File is likely still being written
                    Console.WriteLine($"Video file is locked (attempt {attempt}): {ex.Message}");
                    await Task.Delay(5000 * attempt); // Exponential backoff
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
                // Store video reference before closing page
                IVideo video = null;
                try
                {
                    video = page.Video;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to get video reference: {ex.Message}");
                }

                // Close page and context explicitly
                var context = page.Context;
                var tracePath = $"trace-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
                await context.Tracing.StopAsync(new()
                {
                    Path = tracePath
                });

                AllureApi.AddAttachment("Playwright Trace", "application/zip", tracePath);
                File.Delete(tracePath);

                // Add trace viewer link
                AllureApi.AddAttachment(
                    "🔍 Open Trace Viewer: https://trace.playwright.dev/",
                    "text/uri-list",
                    "trace-viewer.link"
                );
                await page.CloseAsync();
                await context.CloseAsync();

                // Now process the video with proper waiting
                if (video != null && hasFailed)
                {
                    await ProcessVideoWithRetry(video, _scenarioContext.ScenarioInfo.Title, hasFailed);
                }
            }

            // Clean up browser and Playwright
            if (_browser != null)
                await _browser.CloseAsync();

            if (_playwright != null)
                _playwright.Dispose();
        }

        [Given("I navigate to OpenWeatherMap homepage")]
        public async Task GivenINavigateToOpenWeatherMapHomepage()
        {
            var page = _scenarioContext["Page"] as IPage;
            await page.GotoAsync("https://openweathermap.org/");

            // Handle cookie consent popup if it appears
            var cookieAcceptButton = await page.QuerySelectorAsync("button#stick-footer-panel__link");
            if (cookieAcceptButton != null)
            {
                await cookieAcceptButton.ClickAsync();
                await page.WaitForTimeoutAsync(1000); // Wait for animation
            }

            //Wait for search results to load
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
               new PageWaitForLoadStateOptions { Timeout = 10000 });

        }

        [When("I search for {string} in the weather search")]
        public async Task WhenISearchForCityInWeatherSearch(string cityName)
        {
            var page = _scenarioContext["Page"] as IPage;

            // Find search box and enter city name
            await page.FillAsync("input[placeholder='Search city']", cityName);
            await page.ClickAsync("button[type='submit']");
           

            // Store the search term for later assertions
            _scenarioContext["SearchTerm"] = cityName;
        }

        [When("Click on first result")]
        public async Task WhenClickOnFirstResult()
        {
            var page = _scenarioContext["Page"] as IPage;
            await page.WaitForTimeoutAsync(500); // Wait for dropdown to appear
            await page.WaitForSelectorAsync(".search-dropdown-menu li:first-child",
               new PageWaitForSelectorOptions
               {
                   State = WaitForSelectorState.Visible,
                   Timeout = 5000
               });
            await page.ClickAsync(".search-dropdown-menu li:first-child");
            // Wait for search results to load
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
                new PageWaitForLoadStateOptions { Timeout = 10000 });
            await page.WaitForResponseAsync(response =>
            response.Url.StartsWith("https://api.openweathermap.org/data/2.5/onecall") && response.Status == 200);

        }


        [Then("I should see weather details for {string}")]
        public async Task ThenIShouldSeeWeatherDetailsForCity(string cityName)
        {
            var page = _scenarioContext["Page"] as IPage;

            // Wait for the search results to display
            await page.WaitForSelectorAsync(".current-container", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            // Verify city name is displayed in results
            var cityElement = await page.QuerySelectorAsync(".current-container h2");
            var cityText = await cityElement.TextContentAsync();
            cityText.ShouldContain(cityName, (Case)StringComparison.OrdinalIgnoreCase);

            // Verify weather data elements are present
            var temperatureElement = await page.QuerySelectorAsync(".current-temp");
            temperatureElement.ShouldNotBeNull();

            // Take screenshot and add to Allure report
            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/success-search-{cityName}-{DateTime.Now:yyyyMMdd-HHmmss}.png"
            });
            AllureApi.AddAttachment(
                $"Weather Details for {cityName}",
                "image/png",
                screenshotBytes
            );
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

            // Verify not found message is displayed
            var notFoundElement = await page.QuerySelectorAsync("div.sub.not-found.notFoundOpen");
            var notFoundText = await notFoundElement.TextContentAsync();
            notFoundText.ShouldContain("not found", (Case)StringComparison.OrdinalIgnoreCase);

            // Take screenshot and add to Allure report
            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/no-results-search-{DateTime.Now:yyyyMMdd-HHmmss}.png"
            });
            AllureApi.AddAttachment(
                $"No results search",
                "image/png",
                screenshotBytes
            );
        }
    }
}
