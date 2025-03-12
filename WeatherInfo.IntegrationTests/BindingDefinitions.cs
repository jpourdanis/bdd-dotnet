using Allure.Net.Commons;
using Microsoft.AspNetCore.Mvc;
using Reqnroll;
using Shouldly;
using System.Net;
using WeatherInfo.Responses;

namespace WeatherInfo.IntegrationTests
{

    [Binding]
    public class BindingDefinitions
    {
        readonly ScenarioContext scenarioContext;
        private readonly HttpClient _client;
        private static readonly CustomWebApplicationFactory _factory = new CustomWebApplicationFactory();

        public BindingDefinitions(ScenarioContext scenarioContext,CustomWebApplicationFactory factory)
        {
            this.scenarioContext = scenarioContext;
            _client = _factory.CreateClient();
        }
           
        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            AllureLifecycle.Instance.CleanupResultDirectory();

            await _factory.InitializeAsync();
        }

        [AfterTestRun]
        public static async Task AfterScenario()
        {
            await _factory.DisposeAsync();
        }

        [Given("I have a valid city name")]
        public void GivenIHaveAValidCityName()
        {
            scenarioContext["city"] = CustomWebApplicationFactory.ExistingCity;
        }

        [Given("I have an invalid city name")]
        public void GivenIHaveAnInvalidCityName()
        {
            scenarioContext["city"] = CustomWebApplicationFactory.NotExistingCity;
        }

        [Given("I have an empty city name")]
        public void GivenIHaveAnEmptyCityName()
        {
            scenarioContext["city"] = string.Empty;
        }

        [Given("Step with table")]
        public void GivenStepWithTable(DataTable dataTable)
        {
            throw new PendingStepException();
        }


        [When("I request the weather information")]
        public async Task WhenIRequestTheWeatherInformation()
        {
            var city = scenarioContext["city"] as string;
            var response = await _client.GetAsync($"weatherinfo?city={city}");
            scenarioContext["response"] = response;
        }


        [Then("the response should be a bad request with validation error")]
        public async Task ThenTheResponseShouldBeABadRequestWithValidationError()
        {
            var response = scenarioContext["response"] as HttpResponseMessage;
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            errorResponse.ShouldNotBeNull();
            errorResponse.Title.ShouldNotBeNull();
            errorResponse.Title.ShouldContain("One or more validation errors occurred.");
            AllureApi.AddAttachment("Error message","text/plain", System.Text.Encoding.UTF8.GetBytes(errorResponse.Title));
        }

        [Then("the response should be a bad request with no matching location error")]
        public async Task ThenTheResponseShouldBeABadRequestWithNoMatchingLocationError()
        {
            var response = scenarioContext["response"] as HttpResponseMessage;
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            errorResponse.ShouldNotBeNull();
            errorResponse.Title.ShouldNotBeNull();
            errorResponse.Title.ShouldContain("No matching location found.");
            AllureApi.AddAttachment("Error Message","text/plain",System.Text.Encoding.UTF8.GetBytes(errorResponse.Title));
        }

        [Then("the response should be successful with weather information")]
        public async Task ThenTheResponseShouldBeSuccessfulWithWeatherInformation()
        {
            var response = scenarioContext["response"] as HttpResponseMessage;
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var weatherInfoResponse = await response.Content.ReadFromJsonAsync<WeatherInfoResponse>();
            weatherInfoResponse.ShouldNotBeNull();
            AllureApi.AddAttachment("Full Response","application/json",System.Text.Encoding.UTF8.GetBytes(response.Content.ReadAsStringAsync().Result));
        }
    }
}
