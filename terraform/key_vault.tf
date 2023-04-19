##### KEY VAULT
resource "azurerm_key_vault" "kv" {
  name                        = "kv-${var.base_name}-${var.environment}-${format("%02s",var.base_instance)}"
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get",
    ]

    secret_permissions = [
      "Get",
      "Set",
      "List",
    ]

    storage_permissions = [
      "Get",
    ]
  }
}

resource random_password sql_password {
  length  = 64
  special = true
}

resource "azurerm_key_vault_secret" "sql_username" {
  name = "sql-username"
  value = var.sql_username
  key_vault_id = azurerm_key_vault.kv.id
}

resource "azurerm_key_vault_secret" "sql_password" {
  name = "sql-password"
  value = random_password.sql_password.result
  key_vault_id = azurerm_key_vault.kv.id
}