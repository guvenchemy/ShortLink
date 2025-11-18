# 1. Aşama: Build (Derleme)
#Microsoft'un resmi .NET 8 SDK imajını kullanıyoruz

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

#Proje dosyasını kopyala ve paketleri yükle (Restore)

COPY ["ShortLink.csproj","./"]
RUN dotnet restore "ShortLink.csproj"

# Geriye kalan tüm dosyaları kopyala

COPY . .


#Uygulamayı Release modunda derle ve yayınla

RUN dotnet publish "ShortLink.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Runtime ( çalıştırma )
# Sadece çalıştırma dosyalarını içeren daha hafif ASP.NET imajını kullanıyoruz

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app


#Build aşamasında oluşturulan dosyaları buraya al

COPY --from=build /app/publish .

#Konteyner dışarıya 8080 portundan yayın yapacak

EXPOSE 8080

ENTRYPOINT ["dotnet","ShortLink.dll"]