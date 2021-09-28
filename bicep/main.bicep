param resourceGroupName string

// Deploy storage account using module
module storage './storage.bicep' = {
  name: 'storageDeployment'
  scope: resourceGroup(resourceGroupName)
  params: {
    storageAccountName: 'moviematch20210928'
  }
}
