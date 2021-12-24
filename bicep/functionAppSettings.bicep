param functionAppName string
param functionAppNameStaging string
param storageAccountConnectionString string
param appInsightsKey string
param keyVaultUri string
param identityTenantId string
param identityClientId string
param identityInstance string

var settingsProperties = {
  APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsKey
  AzureWebJobsStorage: storageAccountConnectionString
  FUNCTIONS_EXTENSION_VERSION: '~3'
  FUNCTIONS_WORKER_RUNTIME: 'dotnet'
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionString
  WEBSITE_CONTENTSHARE: functionAppName
  KeyVaultUri: keyVaultUri
  AzureAD: {
    TenantId: identityTenantId
    ClientId: identityClientId
    Instance: identityInstance
  }
}

resource functionAppSettings 'Microsoft.Web/sites/config@2021-01-15' = {
  name: '${functionAppName}/appsettings'
  properties: settingsProperties
}

resource functionAppSettingsStaging 'Microsoft.Web/sites/slots/config@2021-02-01' = {
  name: '${functionAppNameStaging}/appsettings'
  properties: settingsProperties
}
