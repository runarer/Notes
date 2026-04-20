# Build the api
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /App

# Copy the solution file and the Directory.Packages.props file
COPY *.slnx ./
COPY *.props ./


#Copy project file and restore
COPY Source/NotesWeb/*.csproj Source/NotesWeb/
RUN dotnet restore Source/NotesWeb/*.csproj

# Copy source code
COPY . .
WORKDIR /App/Source/NotesWeb

# Compile api
RUN dotnet publish -c Release -o ../../out

# Runtime for api
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
COPY --from=build /App/out .

# Entrypoint, for starting api
ENTRYPOINT ["dotnet","NotesWeb.dll"]