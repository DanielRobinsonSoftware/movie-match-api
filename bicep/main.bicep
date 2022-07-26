param appName string
param location string = resourceGroup().location
param corsProduction string
param corsStaging string

@secure()
param userObjectId string
@secure()
param movieDBAccessToken string
@secure()
param identityInstance string
@secure()
param apiApplicationId string

var uniqueSuffix = uniqueString(resourceGroup().id)
var globallyUniqueName = toLower('${appName}${uniqueSuffix}')

// Storage account and keyvault names must be no longer than 24 characters, lowercase and globally unique
var shortLength = min(length(globallyUniqueName), 24)
var shortGloballyUniqueName = substring(globallyUniqueName, 0, shortLength)
var functionAppNameStaging = '${appName}/staging'

module storageAccountModule 'storageAccount.bicep' = {
  name: 'storageAccountModule'
  scope: resourceGroup()
  params: {
    storageAccountName: shortGloballyUniqueName
    location: location
  }
}

module functionAppModule 'functionApp.bicep' = {
  name: 'functionAppModule'
  scope: resourceGroup()
  params: {
    appInsightsName: globallyUniqueName
    hostingPlanName: globallyUniqueName
    functionAppName: appName
    functionAppNameStaging: functionAppNameStaging
    corsProduction: corsProduction
    corsStaging: corsStaging
    location: location
  }
  dependsOn:[
    storageAccountModule
  ]
}

module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultModule'
  params: {
    keyVaultName: shortGloballyUniqueName
    tenantId: subscription().tenantId
    ownerObjectId: userObjectId
    targetObjectIds: [
      functionAppModule.outputs.principalId
      functionAppModule.outputs.stagingPrincipalId
    ]
    movieDBAccessToken: movieDBAccessToken
    location: location
  }
  dependsOn:[
    functionAppModule
  ]
}

module functionAppSettingsModule 'functionAppSettings.bicep' = {
  name: 'functionAppSettingsModule'
  params: {
    functionAppName: appName
    functionAppNameStaging: functionAppNameStaging
    appInsightsKey: functionAppModule.outputs.appInsightsKey
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    identityTenantId: subscription().tenantId
    identityClientId: subscription().subscriptionId
    identityInstance: identityInstance
    apiApplicationId: apiApplicationId
    storageAccountName: shortGloballyUniqueName
    storageAccountId: storageAccountModule.outputs.storageAccountId
    storageAccountApiVersion: storageAccountModule.outputs.storageAccountApiVersion
  }
  dependsOn:[
    functionAppModule
    keyVaultModule
    storageAccountModule
  ]
}
