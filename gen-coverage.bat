nuget install ReportGenerator -Version 3.1.2 -OutputDirectory packages
nuget install OpenCover -Version 4.6.519 -OutputDirectory packages
nuget install Codecov -Version 1.0.3 -OutputDirectory packages

cd "Decorator.Tests"
del "coverage.opencover.xml"

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

cd ..

packages\ReportGenerator.3.1.2\tools\ReportGenerator.exe ^
	-reports:Decorator.Tests\coverage.opencover.xml ^
	-targetdir:Reports\Decorator\