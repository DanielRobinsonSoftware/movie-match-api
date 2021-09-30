param appName string

var globallyUniqueName = '${appName}${uniqueString(resourceGroup().id)}'

// Storage account names must be between 3 and 24 characters in length and may contain numbers and lowercase letters only. Storage account name must be unique within Azure.
var storageAccountName = '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}' 


module functionAppModule 'functionApp.bicep' = {
  name: 'functionAppModule'
  scope: resourceGroup()
  params: {
    storageAccountName: storageAccountName
    appInsightsName: globallyUniqueName
    hostingPlanName: globallyUniqueName
    functionAppName: appName
  }
}

module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultModule'
  params: {
    keyVaultName: globallyUniqueName
    tenantId: functionAppModule.outputs.tenantId
    principalId: functionAppModule.outputs.principalId
  }
  dependsOn:[
    functionAppModule
  ]
}
