# Briscola

## Commands to create the projects

```bash
cd
mkdir Briscola
cd Briscola

dotnet new sln -n Briscola
dotnet new classlib -n Briscola.Domain
dotnet new webapi -n Briscola.Server
dotnet new xunit -n Briscola.Tests

# Add projects to solution
dotnet sln Briscola.sln add Briscola.Domain/Briscola.Domain.csproj
dotnet sln Briscola.sln add Briscola.Server/Briscola.Server.csproj
dotnet sln Briscola.sln add Briscola.Tests/Briscola.Tests.csproj

# Add references to solution
dotnet add Briscola.Server/Briscola.Server.csproj reference Briscola.Domain/Briscola.Domain.csproj
dotnet add Briscola.Tests/Briscola.Tests.csproj reference Briscola.Domain/Briscola.Domain.csproj
```

