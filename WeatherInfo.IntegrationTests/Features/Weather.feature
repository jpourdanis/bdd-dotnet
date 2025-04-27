@labels @core @epic:v1.0 @owner:BabisKarypidis
Feature: Weather

@success @api @link:http://localhost/weatherinfo?city=Thessaloniki
Scenario: Request Weather Info for Existing City Name
	Given I have a valid city name
    When I request the weather information
    Then the response should be successful with weather information

@negative @api @link:http://localhost/weatherinfo?city=
Scenario: Request Weather Info for Non Existing City Name
	Given I have an invalid city name
    When I request the weather information
    Then the response should be a bad request with no matching location error

@negative @api @link:http://localhost/weatherinfo?city=NotExistingCity
Scenario: Request Weather Info for Empty City Name
	Given I have an empty city name
	When I request the weather information
	Then the response should be a bad request with validation error
