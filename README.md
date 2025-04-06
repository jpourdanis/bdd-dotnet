# BDD .NET – Beer-Driven Development Example in .NET

This repository demonstrates how to implement **Behavior-Driven Development (BDD)** in a .NET environment using C# and Gherkin. It contains a sample application (`WeatherInfo`) and corresponding integration tests (`WeatherInfo.IntegrationTests`) that define behavior using Gherkin feature files and test them using Reqnroll.

📄 **Live Test Report**  
You can view the generated Allure test report here:  
👉 [https://jpourdanis.github.io/bdd-dotnet](https://jpourdanis.github.io/bdd-dotnet)

---

## ✨ Features

- ✅ BDD tests written in Gherkin syntax
- ✅ Integration testing with Reqnroll
- ✅ Simple .NET example app for demo purposes
- ✅ HTML Allure test report generation and publishing to GitHub Pages

---

## 🛠 Prerequisites

Make sure you have the following installed:

- [.NET SDK 9.0 or later](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- A C# IDE like [Visual Studio](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)

---

## 🚀 Running the Tests Locally

1. **Clone the repository**

   ```bash
   git clone https://github.com/jpourdanis/bdd-dotnet.git
   cd bdd-dotnet
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Run the BDD Integration Tests**

   Navigate to the integration tests project and run:

   ```bash
   cd WeatherInfo.IntegrationTests
   dotnet test
   ```

   This will run all tests defined in the `.feature` files using Reqnroll.

---

## 📊 Viewing the Test Report

After running the tests, an Allure HTML report is generated with attachments.

To view the report locally :

   ```bash
   cd WeatherInfo.IntegrationTests/bin/Debug/net9.0
   allure generate
   allure serve
   ```

Alternatively, you can view the latest report hosted on GitHub Pages here:  
👉 **[https://jpourdanis.github.io/bdd-dotnet](https://jpourdanis.github.io/bdd-dotnet)**

---

## 📜 License

This project is licensed under the [GNU GPLv3](LICENSE).
