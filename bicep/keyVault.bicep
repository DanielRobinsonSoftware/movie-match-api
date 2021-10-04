param keyVaultName string
param tenantId string
param ownerObjectId string
param targetObjectId  string
param movieDBAccessToken string

var location = resourceGroup().location

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: tenantId
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: ownerObjectId
        permissions: {
          secrets: [
            'get'
            'backup'
            'delete'
            'list'
            'purge'
            'recover'
            'restore'
            'set'
          ]
        }
      }
      {
        tenantId: tenantId
        objectId: targetObjectId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
    sku: {
      family: 'A'
      name: 'standard'
    }
  }
}

resource movieDBAccessTokenSecret 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  name: '${keyVaultName}/MovieDBAccessToken'
  properties: {
    value: movieDBAccessToken
  }
  dependsOn: [
    keyVault
  ]
}

output keyVaultUri string = keyVault.properties.vaultUri
