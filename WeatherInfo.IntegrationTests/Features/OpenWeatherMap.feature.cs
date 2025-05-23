// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace WeatherInfo.IntegrationTests.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Xunit.TraitAttribute("Category", "labels")]
    [Xunit.TraitAttribute("Category", "core")]
    [Xunit.TraitAttribute("Category", "epic:v1.0")]
    [Xunit.TraitAttribute("Category", "owner:JohnPourdanis")]
    public partial class OpenWeatherMapFeature : object, Xunit.IClassFixture<OpenWeatherMapFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private static global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "labels",
                "core",
                "epic:v1.0",
                "owner:JohnPourdanis"};
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "OpenWeatherMap.feature"
#line hidden
        
        public OpenWeatherMapFeature(OpenWeatherMapFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }
        
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(null, global::Reqnroll.xUnit.ReqnrollPlugin.XUnitParallelWorkerTracker.Instance.GetWorkerId());
            global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "OpenWeatherMap", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
            await testRunner.OnFeatureStartAsync(featureInfo);
        }
        
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
            string testWorkerId = testRunner.TestWorkerId;
            await testRunner.OnFeatureEndAsync();
            testRunner = null;
            global::Reqnroll.xUnit.ReqnrollPlugin.XUnitParallelWorkerTracker.Instance.ReleaseWorker(testWorkerId);
        }
        
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
        }
        
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
        {
            await this.TestInitializeAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await this.TestTearDownAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Search Weather By Valid City")]
        [Xunit.TraitAttribute("FeatureTitle", "OpenWeatherMap")]
        [Xunit.TraitAttribute("Description", "Search Weather By Valid City")]
        [Xunit.TraitAttribute("Category", "passed")]
        [Xunit.TraitAttribute("Category", "ui")]
        [Xunit.TraitAttribute("Category", "search")]
        [Xunit.TraitAttribute("Category", "link:https://openweathermap.org/city/264371")]
        public async System.Threading.Tasks.Task SearchWeatherByValidCity()
        {
            string[] tagsOfScenario = new string[] {
                    "passed",
                    "ui",
                    "search",
                    "link:https://openweathermap.org/city/264371"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Search Weather By Valid City", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 5
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 6
    await testRunner.GivenAsync("I navigate to OpenWeatherMap homepage", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 7
    await testRunner.WhenAsync("I search for \"Athens\" in the weather search", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 8
    await testRunner.AndAsync("Click on first result", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 9
    await testRunner.ThenAsync("I should see weather details for \"Athens\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Search Weather By Invalid City")]
        [Xunit.TraitAttribute("FeatureTitle", "OpenWeatherMap")]
        [Xunit.TraitAttribute("Description", "Search Weather By Invalid City")]
        [Xunit.TraitAttribute("Category", "passed")]
        [Xunit.TraitAttribute("Category", "ui")]
        [Xunit.TraitAttribute("Category", "negative")]
        [Xunit.TraitAttribute("Category", "search")]
        public async System.Threading.Tasks.Task SearchWeatherByInvalidCity()
        {
            string[] tagsOfScenario = new string[] {
                    "passed",
                    "ui",
                    "negative",
                    "search"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Search Weather By Invalid City", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 13
    await testRunner.GivenAsync("I navigate to OpenWeatherMap homepage", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 14
    await testRunner.WhenAsync("I search for \"NonExistentCity123456\" in the weather search", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 15
    await testRunner.ThenAsync("I should see no results found message", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : object, Xunit.IAsyncLifetime
        {
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
            {
                await OpenWeatherMapFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await OpenWeatherMapFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion
