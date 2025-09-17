param(
    [string]$ResourceGroupName = "rg-AzureFunctionCosmosTemplate",
    [string]$Location = "East US"
)

Write-Host "======================================================"
Write-Host "Deployment Automatico - AzureFunctionCosmosTemplate"
Write-Host "======================================================"

try {
    # Step 1: Verificar Azure CLI
    Write-Host "[Step 1] Verificando Azure CLI..."
    $account = az account show 2>$null
    if (!$account) {
        Write-Host "Haciendo login a Azure..."
        az login
    }
    $subscription = az account show --query "name" -o tsv
    Write-Host "[SUCCESS] Subscription: $subscription"

    # Step 2: Build
    Write-Host "[Step 2] Building aplicacion..."
    dotnet restore
    dotnet build --configuration Release --no-restore
    dotnet test Tests/AzureFunctionCosmosTemplate.UnitTests --configuration Release --no-build --logger "console;verbosity=minimal"
    Write-Host "[SUCCESS] Build completado!"

    # Step 3: Crear Resource Group
    Write-Host "[Step 3] Creando Resource Group..."
    az group create --name $ResourceGroupName --location $Location
    Write-Host "[SUCCESS] Resource Group creado"

    # Step 4: Deploy infrastructure
    Write-Host "[Step 4] Desplegando infraestructura..."
    $deploymentName = "deployment-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    
    $result = az deployment group create `
        --resource-group $ResourceGroupName `
        --name $deploymentName `
        --template-file "./Infrastructure/main.bicep" `
        --parameters appName="AzureFunctionCosmosTemplate" `
        --parameters location="$Location" `
        --parameters cosmosDbAccountName="AzureFunctionCosmosTemplate" `
        --parameters cosmosDatabaseName="AzureFunctionCosmosTemplate" `
        --output json | ConvertFrom-Json
    
    $functionAppName = $result.properties.outputs.functionAppName.value
    $functionAppUrl = $result.properties.outputs.functionAppUrl.value
    
    Write-Host "[SUCCESS] Infraestructura desplegada!"
    Write-Host "Function App: $functionAppName"
    Write-Host "URL: $functionAppUrl"

    # Step 5: Deploy codigo
    Write-Host "[Step 5] Desplegando codigo..."
    $publishPath = "./publish"
    dotnet publish "./AzureFunctionCosmosTemplate.Function" --configuration Release --output $publishPath --no-build
    
    $zipPath = "./deploy.zip"
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
    Compress-Archive -Path "$publishPath/*" -DestinationPath $zipPath -Force
    
    az functionapp deployment source config-zip --resource-group $ResourceGroupName --name $functionAppName --src $zipPath
    
    Write-Host "[SUCCESS] Codigo desplegado!"

    # Resultado final
    Write-Host ""
    Write-Host "============================================"
    Write-Host "DEPLOYMENT COMPLETADO!"
    Write-Host "============================================"
    Write-Host "Function URL: $functionAppUrl"
    Write-Host "Swagger UI: $functionAppUrl/api/swagger/ui"
    Write-Host "Resource Group: $ResourceGroupName"
    Write-Host ""
    Write-Host "Para demo:"
    Write-Host "1. Abre Swagger UI en tu browser"
    Write-Host "2. Prueba GET /api/users"
    Write-Host "3. Crea usuario con POST /api/users"
    Write-Host "4. Prueba GET /api/users otra vez"
}
catch {
    Write-Host "[ERROR] Fallo: $($_.Exception.Message)"
    exit 1
}
finally {
    # Cleanup
    if (Test-Path "./publish") { Remove-Item "./publish" -Recurse -Force }
    if (Test-Path "./deploy.zip") { Remove-Item "./deploy.zip" -Force }
}
