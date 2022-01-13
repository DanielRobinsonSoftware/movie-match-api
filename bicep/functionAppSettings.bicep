param functionAppName string
param functionAppNameStaging string

@secure()
param appInsightsKey string
@secure()
param keyVaultUri string
@secure()
param identityTenantId string
@secure()
param identityClientId string
@secure()
param identityInstance string
@secure()
param apiApplicationId string

param storageAccountName string
param storageAccountId string
param storageAccountApiVersion string

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccountId, storageAccountApiVersion).keys[0].value}'

var settingsProperties = {
  APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsKey
  AzureWebJobsStorage: storageAccountConnectionString
  FUNCTIONS_EXTENSION_VERSION: '~3'
  FUNCTIONS_WORKER_RUNTIME: 'dotnet'
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionString
  WEBSITE_CONTENTSHARE: functionAppName
  KeyVaultUri: keyVaultUri  
  AzureADTenantId: identityTenantId
  AzureADClientId: identityClientId
  AzureADInstance: identityInstance
  ApiApplicationId: apiApplicationId
}

resource functionAppSettings 'Microsoft.Web/sites/config@2021-01-15' = {
  name: '${functionAppName}/appsettings'
  properties: settingsProperties
}

resource functionAppSettingsStaging 'Microsoft.Web/sites/slots/config@2021-02-01' = {
  name: '${functionAppNameStaging}/appsettings'
  properties: settingsProperties
}
