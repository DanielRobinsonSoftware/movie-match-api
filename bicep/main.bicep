param appName string
param userObjectId string
param movieDBAccessToken string

var uniqueSuffix = uniqueString(resourceGroup().id)
var globallyUniqueName = toLower('${appName}${uniqueSuffix}')

// Storage account and keyvault names must be no longer than 24 characters, lowercase and globally unique
var shortLength = min(length(globallyUniqueName), 24)
var shortGloballyUniqueName = substring(globallyUniqueName, 0, shortLength)


module functionAppModule 'functionApp.bicep' = {
  name: 'functionAppModule'
  scope: resourceGroup()
  params: {
    storageAccountName: shortGloballyUniqueName
    appInsightsName: globallyUniqueName
    hostingPlanName: globallyUniqueName
    functionAppName: appName
  }
}

module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultModule'
  params: {
    keyVaultName: shortGloballyUniqueName
    tenantId: subscription().tenantId
    ownerObjectId: userObjectId
    targetObjectId: functionAppModule.outputs.principalId
    movieDBAccessToken: movieDBAccessToken
  }
  dependsOn:[
    functionAppModule
  ]
}

module functionAppSettingsModule 'functionAppSettings.bicep' = {
  name: 'functionAppSettingsModule'
  params: {
    functionAppName: appName
    storageAccountConnectionString: functionAppModule.outputs.storageAccountConnectionString
    appInsightsKey: functionAppModule.outputs.appInsightsKey
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
  }
  dependsOn:[
    functionAppModule
    keyVaultModule
  ]
}
