#!/bin/sh

sudo dotnet tool install -g dotnet-reportgenerator-globaltool
cd tests/unittests/sampletest && dotnet test --logger 'trx;LogFileName=TestResults.trx' --logger 'xunit;LogFileName=TestResults.xml' --results-directory ./BuildReports/UnitTests /p:CollectCoverage=true /p:CoverletOutput=BuildReports/Coverage/ /p:CoverletOutputFormat=cobertura /p:Exclude='[xunit.*]*'
reportgenerator -reports:BuildReports/Coverage/coverage.cobertura.xml -targetdir:BuildReports/Coverage -reporttypes:"HTML;HTMLSummary"
cd ../../../
chmod +x run-tests.sh
