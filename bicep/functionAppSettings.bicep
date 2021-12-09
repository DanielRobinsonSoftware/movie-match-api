param functionAppName string
param functionAppNameStaging string
param storageAccountConnectionString string
param appInsightsKey string
param keyVaultUri string
param clientId string

var settingsProperties = {
  APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsKey
  AzureWebJobsStorage: storageAccountConnectionString
  FUNCTIONS_EXTENSION_VERSION: '~3'
  FUNCTIONS_WORKER_RUNTIME: 'dotnet'
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionString
  WEBSITE_CONTENTSHARE: functionAppName
  KeyVaultUri: keyVaultUri
  globalValidation: {
    requireAuthentication: true
    unauthenticatedClientAction: 'Return403'
  }
  identityProviders: {
    azureActiveDirectory: {
      enabled: true
      registration: {
        openIdIssuer: 'https://login.microsoftonline.com/${subscription().tenantId}/v2.0'
        clientId: clientId
        clientSecretSettingName: 'AD_IDENTITY_CLIENT_SECRET'
      }
      validation: {
        allowedAudiences: [
            'api://${clientId}'
        ]
      }
      isAutoProvisioned: false
    }
    login: {
      tokenStore: {
        enabled: true
      }
    }
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
