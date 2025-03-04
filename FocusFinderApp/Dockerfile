# Use .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Set the working directory
WORKDIR /FocusFinderApp
# Copy the project files
COPY . .
# Restore and build the project
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Final stage: run the application using the ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /FocusFinderApp
COPY --from=build /FocusFinderApp/out .

# Expose port 80 for the application
EXPOSE 80
# Set the application to listen on IPv4 only
ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["dotnet", "FocusFinderApp.dll"]