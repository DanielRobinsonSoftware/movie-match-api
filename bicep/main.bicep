param appName string
param userObjectId string
param movieDBAccessToken string

var globallyUniqueName = '${appName}${uniqueString(resourceGroup().id)}'

// Storage account and keyvault names must be between 3 and 24 characters in length and globally unique
var shortGloballyUniqueName = '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}' 


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
