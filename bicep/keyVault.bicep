param keyVaultName string
param tenantId string
param ownerObjectId string
param targetObjectIds array

@secure()
param movieDBAccessToken string

var location = resourceGroup().location

var accessPolicyOwner = {
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

var accessPolicyTargetObjects = [for objectId in targetObjectIds: {
  tenantId: tenantId
  objectId: objectId
  permissions: {
    secrets: [
      'get'
      'list'
    ]
  }
}]

var accessPolicies = concat(array(accessPolicyOwner), accessPolicyTargetObjects)

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: tenantId
    accessPolicies: accessPolicies
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
