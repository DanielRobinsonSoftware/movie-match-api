param keyVaultName string
param tenantId string
param objectId string
param movieDBAccessToken string

var location = resourceGroup().location

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: objectId
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
        objectId: objectId
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

resource keyVaultSecrets 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: last(split(keyVault.id, '/'))
  resource movieDBAccessTokenSecret 'secrets' = {
    name: 'movieDBAccessTokenSecret'
    properties: {
      value: movieDBAccessToken
    }
  }
}
