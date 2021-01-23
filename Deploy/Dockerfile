FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./API ./
RUN dotnet restore CheckoutPaymentAPI/CheckoutPaymentAPI.csproj
RUN dotnet publish CheckoutPaymentAPI/CheckoutPaymentAPI.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CheckoutPaymentAPI.dll"];