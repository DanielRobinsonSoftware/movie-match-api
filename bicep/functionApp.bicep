param storageAccountName string
param appInsightsName string
param hostingPlanName string
param functionAppName string
param functionAppNameStaging string

var location = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2021-01-15' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {

    }
  }

  dependsOn: [
    appInsights
    hostingPlan
    storageAccount
  ]
}

resource functionAppStagingSlot 'Microsoft.Web/sites/slots@2021-02-01' = {
  name: functionAppNameStaging
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {

    }
  }

  dependsOn: [
    functionApp
  ]
}

output tenantId string = functionApp.identity.tenantId
output principalId string = functionApp.identity.principalId
output stagingTenantId string = functionAppStagingSlot.identity.tenantId
output stagingPrincipalId string = functionAppStagingSlot.identity.principalId
output storageAccountConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
output appInsightsKey string = appInsights.properties.InstrumentationKey
