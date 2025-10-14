# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto individualmente para aproveitar cache do Docker
COPY syspublicidade.prefeitura.Api/syspublicidade.prefeitura.Api.csproj syspublicidade.prefeitura.Api/
COPY syspublicidade.prefeitura.Application/syspublicidade.prefeitura.Application.csproj syspublicidade.prefeitura.Application/
COPY syspublicidade.prefeitura.Infrastructure/syspublicidade.prefeitura.Infrastructure.csproj syspublicidade.prefeitura.Infrastructure/
COPY syspublicidade.prefeitura.Domain/syspublicidade.prefeitura.Domain.csproj syspublicidade.prefeitura.Domain/

# Restaurar dependências
RUN dotnet restore ./syspublicidade.prefeitura.Api/syspublicidade.prefeitura.Api.csproj

# Copiar todo o restante do código-fonte do repositório
COPY . .

# Publicar o projeto principal (gera DLLs e arquivos prontos)
WORKDIR /src/syspublicidade.prefeitura.Api
RUN dotnet publish syspublicidade.prefeitura.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Etapa de runtime (imagem menor, apenas arquivos publicados)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configurar ambiente e portas
ENV ASPNETCORE_URLS="http://*:8081"
EXPOSE 8080

# Comando de inicialização do container (executa sua API)
ENTRYPOINT ["dotnet", "syspublicidade.prefeitura.Api.dll"]