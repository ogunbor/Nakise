# How to run the test
From the root folder of the project `nikase` in your terminal, change directory (cd) into tests/unittests/{project folder} e.g cd tests/unittests/sampleTests
Run the following in command

```sh
dotnet run test
```

# How to run test coverage and generate test coverage report 
Note: The following Nuget package should be installed on your system as a .NET global tool. Use the `dotnet tool install` command to install the following Nuget package

```sh
dotnet tool install -g dotnet-reportgenerator-globaltool
```

This will be used for data collection from the test runs to generate report as a styled HTML

## Genrating Test reports and coverage on Windows
In the root folder of the project `nikase` in your termial run the following command 
```sh
./run-test.bat
```

## Generating Test report and coverage on Mac/Linux
In the root folder of the project `nikase` in your termial run the following command 
```sh
./run-test.sh
```

## How to view the auto generated test report in a HTML file
In the test project, go to the BuildReports/Coverage folder and open the `index.html` in a browser
For more information, you can follow this link [here](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux)
