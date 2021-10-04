param functionAppName string
param storageAccountConnectionString string
param appInsightsKey string
param keyVaultUri string

resource functionAppSettings 'Microsoft.Web/sites/config@2021-01-15' = {
  name: '${functionAppName}/appsettings'
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsKey
    AzureWebJobsStorage: storageAccountConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionString
    KeyVaultUri: keyVaultUri
  }
}
