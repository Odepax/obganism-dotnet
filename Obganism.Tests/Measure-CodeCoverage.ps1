$ErrorActionPreference = "Stop"

# Setup code coverage tools.

dotnet add package "coverlet.collector"

dotnet tool install "dotnet-reportgenerator-globaltool" `
	--tool-path "./bin/Tools"

# Generate code coverage statistics.

dotnet test `
	--collect "XPlat Code Coverage" `
	--results-directory "./bin/Coverage"

# Generate code coverage HTML report.

./bin/Tools/reportgenerator.exe `
	-reports:./bin/Coverage/*/* `
	-targetdir:./bin/Coverage/Report `
	-historydir:./bin/Coverage/History
