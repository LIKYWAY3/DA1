# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Sao chep file csproj va restore dependencies
COPY ["ASPtestShop/ASPtestShop.csproj", "ASPtestShop/"]
RUN dotnet restore "ASPtestShop/ASPtestShop.csproj"

# Sao chep toan bo source code cua ung dung
COPY ASPtestShop/ ASPtestShop/
WORKDIR "/src/ASPtestShop"

# Build va publish ban Release
RUN dotnet publish "ASPtestShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Tao thu muc luu upload anh san pham
RUN mkdir -p /app/wwwroot/Uploads/products

# Cau hinh cong cho Render
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ASPtestShop.dll"]
