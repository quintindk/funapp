#### SQL
data "azuread_user" "sql_ad_admin" {
  user_principal_name = var.sql_ad_admin
}

resource "azurerm_mssql_server" "sql" {
  name                         = "sql-${var.base_name}-${var.environment}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = azurerm_key_vault_secret.sql_username.value
  administrator_login_password = azurerm_key_vault_secret.sql_password.value

  azuread_administrator {
    login_username = data.azuread_user.sql_ad_admin.user_principal_name
    object_id      = data.azuread_user.sql_ad_admin.object_id
  }

  tags = var.tags
}

resource "azurerm_mssql_database" "sql" {
  name      = "blogging"
  server_id = azurerm_mssql_server.sql.id
  max_size_gb        = 4
  read_scale         = true
  read_replica_count = 1
  sku_name           = "P2"
  zone_redundant     = true

  tags = var.tags
}