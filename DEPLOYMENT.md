# 🚀 Deployment Guide

Guía simplificada para deployar tu Azure Function con Cosmos DB de forma rápida y fácil.

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- Suscripción de Azure activa

## 🎯 Deployment Automático (Recomendado)

### Opción 1: Script Todo-en-Uno ⭐⭐⭐

**Un solo comando hace todo:**

```powershell
.\deploy-now.ps1
```

**¿Qué hace este script?**
- ✅ Verifica que estés logueado en Azure
- ✅ Compila y testea la aplicación
- ✅ Crea Resource Group: `rg-AzureFunctionCosmosTemplate`
- ✅ Crea Cosmos DB: `azurefunctioncosmostemplate`
- ✅ Crea Function App: `azurefunctioncosmostemplate`
- ✅ Configura connection strings automáticamente
- ✅ Deploya el código
- ✅ Te da las URLs para probar

### Opción 2: Bicep Template (Para más control)

```bash
# Crear Resource Group
az group create --name "rg-AzureFunctionCosmosTemplate" --location "East US"

# Deployar infraestructura
az deployment group create \
  --resource-group "rg-AzureFunctionCosmosTemplate" \
  --template-file "./Infrastructure/main.bicep" \
  --parameters "./Infrastructure/parameters.json"

# Deployar código (manual)
dotnet publish "./AzureFunctionCosmosTemplate.Function" --configuration Release --output "./publish"
Compress-Archive -Path "./publish/*" -DestinationPath "./deploy.zip" -Force
az functionapp deployment source config-zip --resource-group "rg-AzureFunctionCosmosTemplate" --name "[function-app-name]" --src "./deploy.zip"
```

## 🔧 Setup Inicial

### 1. Login a Azure
```bash
az login
```

### 2. Verificar Subscription
```bash
az account show
```

### 3. Ejecutar Deployment
```powershell
.\deploy-now.ps1
```

## 🌐 URLs de tu Aplicación

Después del deployment tendrás:

- **Function App**: `https://azurefunctioncosmostemplate.azurewebsites.net`
- **Swagger UI**: `https://azurefunctioncosmostemplate.azurewebsites.net/api/swagger/ui`
- **API Base**: `https://azurefunctioncosmostemplate.azurewebsites.net/api`

## 📚 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| GET | `/api/users/email/{email}` | Get user by email |
| GET | `/api/users/active` | Get all active users |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update an existing user |
| DELETE | `/api/users/{id}` | Delete a user |

## 🎯 Demo Instructions

### 1. Abrir Swagger UI
Navega a: `https://azurefunctioncosmostemplate.azurewebsites.net/api/swagger/ui`

### 2. Probar GET /api/users
- Debería devolver un array vacío `[]`

### 3. Crear un Usuario (POST /api/users)
Usar este JSON:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "isActive": true
}
```

### 4. Verificar Creación
- Probar GET `/api/users` otra vez
- Debería mostrar el usuario creado
- **Nota**: No verás `id`, `createdAt`, etc. porque usamos DTOs limpios

### 5. Probar Otros Endpoints
- GET `/api/users/email/john@example.com`
- GET `/api/users/active`
- PUT `/api/users/{id}` (usar el ID del usuario creado)

## 🗄️ Base de Datos

### Creación Automática
- ✅ **Database**: Se crea automáticamente al primer uso
- ✅ **Container**: Se crea automáticamente con partition key `/partitionKey`
- ✅ **No setup manual**: Todo es automático

### Configuración
- **Cosmos DB Account**: `azurefunctioncosmostemplate`
- **Database Name**: `AzureFunctionCosmosTemplate`
- **Container Name**: `users`
- **Partition Key**: `/partitionKey`
- **Mode**: Serverless (pay-per-use)

## 🔧 Desarrollo Local

### Para trabajar localmente con BD de Azure
Tu `local.settings.json` ya está configurado con:
```json
{
  "Values": {
    "CosmosDbConnection": "[connection-string-de-azure]",
    "CosmosDbName": "AzureFunctionCosmosTemplate"
  }
}
```

### Para trabajar con emulador local
Cambia a:
```json
{
  "Values": {
    "CosmosDbConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "CosmosDbName": "AzureFunctionCosmosTemplate"
  }
}
```

## 🧪 Testing

```bash
# Ejecutar unit tests
dotnet test Tests/AzureFunctionCosmosTemplate.UnitTests

# Ejecutar con cobertura
dotnet test Tests/AzureFunctionCosmosTemplate.UnitTests --collect:"XPlat Code Coverage"
```

## 🗂️ Archivos Importantes

### Para Deployment
- `deploy-now.ps1` - Script de deployment automático
- `Infrastructure/main.bicep` - Template de infraestructura
- `Infrastructure/parameters.json` - Configuración

### Para Desarrollo
- `AzureFunctionCosmosTemplate.Function/local.settings.json` - Configuración local
- `Tests/` - Proyecto de unit tests

## 🆘 Troubleshooting

### Function App no responde
```bash
# Verificar logs
az functionapp log tail --name "azurefunctioncosmostemplate" --resource-group "rg-AzureFunctionCosmosTemplate"
```

### Problemas de conexión a Cosmos DB
```bash
# Verificar configuración
az functionapp config appsettings list --name "azurefunctioncosmostemplate" --resource-group "rg-AzureFunctionCosmosTemplate"
```

### Re-deployar código
```bash
dotnet publish "./AzureFunctionCosmosTemplate.Function" --configuration Release --output "./publish"
Compress-Archive -Path "./publish/*" -DestinationPath "./deploy.zip" -Force
az functionapp deployment source config-zip --resource-group "rg-AzureFunctionCosmosTemplate" --name "azurefunctioncosmostemplate" --src "./deploy.zip"
```

## 🧹 Cleanup

Para eliminar todos los recursos:
```bash
az group delete --name "rg-AzureFunctionCosmosTemplate" --yes --no-wait
```

## 🎉 ¡Listo para Demo!

Tu aplicación está funcionando en:
- **Swagger UI**: https://azurefunctioncosmostemplate.azurewebsites.net/api/swagger/ui
- **API**: https://azurefunctioncosmostemplate.azurewebsites.net/api/users

**Características destacadas para mostrar en demo:**
- ✅ Creación automática de BD y contenedores
- ✅ APIs RESTful completas con DTOs seguros
- ✅ Documentación OpenAPI/Swagger automática
- ✅ Validaciones automáticas
- ✅ Arquitectura limpia y escalable