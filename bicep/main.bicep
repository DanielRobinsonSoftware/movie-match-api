param appName string

module functionAppModule './functionApp.bicep' = {
  name: 'functionAppModule'
  scope: resourceGroup()
  params: {
    storageAccountName: '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}' 
    appInsightsName: '${appName}${uniqueString(resourceGroup().id)}'
    hostingPlanName: '${appName}${uniqueString(resourceGroup().id)}'
    functionAppName: appName
  }
}
