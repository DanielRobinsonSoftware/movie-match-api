// Deploy storage account using module
module storage './storage.bicep' = {
  name: 'storageDeployment'
  scope: resourceGroup()
  params: {
    storageAccountName: 'moviematch20210928'
  }
}
