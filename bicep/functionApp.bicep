param appInsightsName string
param hostingPlanName string
param functionAppName string
param functionAppNameStaging string
param location string

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
  kind: 'linux'
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
  properties:{
    reserved: true
  }
}

resource functionApp 'Microsoft.Web/sites@2021-01-15' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
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
  ]
}

resource functionAppStagingSlot 'Microsoft.Web/sites/slots@2021-02-01' = {
  name: functionAppNameStaging
  location: location
  kind: 'functionapp,linux'
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
output appInsightsKey string = appInsights.properties.InstrumentationKey
