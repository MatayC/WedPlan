# Multi-Stage-Build für die WedPlan Blazor Web App (.NET 10, Interactive Server).
# Stage 1: Build & Publish mit dem .NET SDK.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Projektdatei zuerst kopieren, damit die Restore-Schicht gecacht werden kann.
COPY WedPlan/WedPlan.csproj WedPlan/
RUN dotnet restore WedPlan/WedPlan.csproj

# Restlichen Quellcode kopieren und in Release veröffentlichen.
COPY WedPlan/ WedPlan/
RUN dotnet publish WedPlan/WedPlan.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Schlankes Runtime-Image (nur ASP.NET Laufzeit).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render (und ähnliche Plattformen) geben den Port über die Umgebungsvariable PORT vor.
# Kestrel wird in Program.cs entsprechend darauf gebunden. Standard 8080 als Fallback.
ENV ASPNETCORE_ENVIRONMENT=Production
# Datei-Überwachung (inotify) deaktivieren – Container haben ein niedriges inotify-Limit,
# das sonst beim Start zu "inotify instances reached" führt. In Produktion nicht benötigt.
ENV DOTNET_hostBuilder__reloadConfigOnChange=false
EXPOSE 8080

ENTRYPOINT ["dotnet", "WedPlan.dll"]
