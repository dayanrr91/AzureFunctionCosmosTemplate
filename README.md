# Azure Function Cosmos DB Template

A comprehensive template for building Azure Functions with Cosmos DB integration, following clean architecture principles and best practices.

## 🏗️ Architecture Overview

This project implements a clean architecture pattern with the following layers:

- **AzureFunctionCosmosTemplate.Function**: HTTP triggers and API endpoints
- **AzureFunctionCosmosTemplate.Application**: Business logic and services
- **AzureFunctionCosmosTemplate.Domain**: Entities, DTOs, and domain models
- **AzureFunctionCosmosTemplate.Repository**: Data access layer with Cosmos DB integration

## 🚀 Features

- ✅ **Clean Architecture**: Proper separation of concerns across layers
- ✅ **Automatic Database Creation**: Creates database and containers automatically if they don't exist
- ✅ **DTO Pattern**: Clean separation between internal entities and API contracts
- ✅ **Extension Methods**: Standardized HTTP response patterns
- ✅ **Comprehensive Logging**: Structured logging throughout the application
- ✅ **OpenAPI Documentation**: Swagger documentation for all endpoints
- ✅ **Dependency Injection**: Proper DI configuration
- ✅ **Error Handling**: Consistent error responses with appropriate HTTP status codes

## 📋 Prerequisites

Before running this project, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- One of the following for Cosmos DB:
  - [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) (recommended for local development)
  - Valid Azure Cosmos DB connection string

## 🛠️ Setup Instructions

### 1. Install Azure Cosmos DB Emulator (Local Development)

**Windows:**
```bash
# Download and install from Microsoft website
# https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator
```

**macOS/Linux:**
```bash
# Use Docker
docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254 \
  -m 3g --cpus=2.0 --name=test-linux-emulator \
  -e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 \
  -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true \
  -e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1 \
  -it mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
```

### 2. Configure Connection Settings

Create a `local.settings.json` file in the `AzureFunctionCosmosTemplate.Function` project:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "CosmosDbName": "UserManagementDB"
  }
}
```

**For Azure Cosmos DB (Production):**
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "your-storage-connection-string",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnection": "your-cosmos-db-connection-string",
    "CosmosDbName": "UserManagementDB"
  }
}
```

### 3. Build and Run

```bash
# Clone the repository
git clone <repository-url>
cd AzureFunctionCosmosTemplate

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Navigate to the Function project
cd AzureFunctionCosmosTemplate.Function

# Run the Azure Function
func start
```

The function will start and be available at `http://localhost:7071`

## 🗄️ Database Auto-Creation

The application automatically handles database setup:

1. **Database Creation**: If the specified database doesn't exist, it will be created automatically
2. **Container Creation**: The `users` container will be created with the appropriate partition key (`/partitionKey`)
3. **No Manual Setup Required**: Simply provide a valid connection string and the application handles the rest

## 📚 API Endpoints

Once running, the following endpoints are available:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| GET | `/api/users/email/{email}` | Get user by email |
| GET | `/api/users/active` | Get all active users |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update an existing user |
| DELETE | `/api/users/{id}` | Delete a user |

### Sample Request (Create User)

```bash
curl -X POST http://localhost:7071/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "isActive": true
  }'
```

## 📖 OpenAPI Documentation

Swagger documentation is available at:
- `http://localhost:7071/api/swagger/ui` (when running locally)

## 🏗️ Project Structure

```
AzureFunctionCosmosTemplate/
├── AzureFunctionCosmosTemplate.Function/     # Azure Functions HTTP triggers
│   ├── HttpTrigger/                          # API endpoints
│   ├── Extensions/                           # HTTP response extensions
│   └── Program.cs                            # Function app configuration
├── AzureFunctionCosmosTemplate.Application/  # Business logic layer
│   ├── Interfaces/                           # Service interfaces
│   ├── Services/                             # Service implementations
│   └── Extensions/                           # DI extensions
├── AzureFunctionCosmosTemplate.Domain/       # Domain layer
│   ├── Entities/                             # Database entities
│   └── DTOs/                                 # Data transfer objects
└── AzureFunctionCosmosTemplate.Repository/   # Data access layer
    ├── Repositories/                         # Repository implementations
    ├── Extensions/                           # Cosmos DB configuration
    └── CosmosRepositoryBase.cs               # Base repository class
```

## 🔧 Configuration Options

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CosmosDbConnection` | Cosmos DB connection string | Required |
| `CosmosDbName` | Database name | Required |
| `FUNCTIONS_WORKER_RUNTIME` | Function runtime | `dotnet-isolated` |

### Cosmos DB Settings

- **Partition Key**: `/partitionKey`
- **Container Name**: `users`
- **Automatic Creation**: Enabled
- **Throughput**: Default (400 RU/s)

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 Deployment

### Azure Deployment

1. Create an Azure Function App
2. Create an Azure Cosmos DB account
3. Configure application settings in Azure Portal
4. Deploy using Azure DevOps, GitHub Actions, or Visual Studio

```bash
# Deploy using Azure Functions Core Tools
func azure functionapp publish <function-app-name>
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Troubleshooting

### Common Issues

**Cosmos DB Emulator Connection Issues:**
- Ensure the emulator is running
- Check that the connection string matches the emulator's endpoint
- Verify firewall settings allow connections to localhost:8081

**Function Not Starting:**
- Ensure .NET 8 SDK is installed
- Check that Azure Functions Core Tools are up to date
- Verify `local.settings.json` exists and is properly configured

**Database/Container Creation Issues:**
- Check that the connection string has sufficient permissions
- Verify the database name doesn't contain invalid characters
- Ensure the Cosmos DB account is accessible

### Getting Help

- Check the [Azure Functions documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- Review [Azure Cosmos DB documentation](https://docs.microsoft.com/en-us/azure/cosmos-db/)
- Open an issue in this repository for project-specific problems