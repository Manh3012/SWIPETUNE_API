	# Start with a base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the SWIPETUNE_API project file
COPY ["SWIPTETUNE_API/SWIPTETUNE_API.csproj", "SWIPTETUNE_API/"]

# Copy the BusinessObject project file
COPY ["BusinessObject/BusinessObject.csproj", "BusinessObject/"]

# Copy the DataAccess project file
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]

# Copy the Repository project file
COPY ["Repository/Repository.csproj", "Repository/"]
# Copy the Client project file

COPY ["SWIPE_TUNE_CLIENT/SWIPE_TUNE_CLIENT.csproj", "SWIPE_TUNE_CLIENT/"]


# Restore the project dependencies
RUN dotnet restore "SWIPTETUNE_API/SWIPTETUNE_API.csproj"

# Copy the source code
COPY . .

# Build the application
RUN dotnet build "SWIPTETUNE_API/SWIPTETUNE_API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "SWIPTETUNE_API/SWIPTETUNE_API.csproj" -c Release -o /app/publish 


# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published files from the build image
COPY --from=build /app/publish .

# Expose the port(s) that the application will listen on
EXPOSE 7049

# Set the entry point for the container
ENTRYPOINT ["dotnet", "SWIPTETUNE_API.dll"]
