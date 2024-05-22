# need to run dotnet tool install -g dotnet-reportgenerator-globaltool
# Documentation:
#     https://reportgenerator.io/usage
#     https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows
#
# removes the previous coverage information     
rm -rf ./coverage

# runs the unit tests and stores coverage info in the coverage directory
dotnet test --collect:"Xplat Code Coverage" --results-directory="./coverage"

# generates a report named index.html from the coverage report generated by Xunit during unit testing
reportgenerator                                    \
    "-reports:coverage/**/*coverage.cobertura.xml" \
    -targetdir:"coveragereport"                    \
    -reporttypes:HtmlInline_AzurePipelines_Dark    \

# uses git to open up report automatically
git web--browse "./coveragereport/index.html"