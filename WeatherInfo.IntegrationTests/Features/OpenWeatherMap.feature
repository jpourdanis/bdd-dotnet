@labels @core @epic:v1.0 @owner:JohnPourdanis
Feature: OpenWeatherMap

@passed @ui @search @link:https://openweathermap.org/city/264371
Scenario: Search Weather By Valid City
    Given I navigate to OpenWeatherMap homepage
    When I search for "Athens" in the weather search
    And Click on first result
    Then I should see weather details for "Athens"

@passed @ui @negative @search 
Scenario: Search Weather By Invalid City
    Given I navigate to OpenWeatherMap homepage
    When I search for "NonExistentCity123456" in the weather search
    Then I should see no results found message
