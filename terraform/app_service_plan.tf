#### APP SERVICE PLAN
resource "azurerm_storage_account" "apps_storage" {
  name                     = "stoapps${var.base_name}${var.environment}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = var.tags
}

resource "azurerm_service_plan" "func_apps" {
  name                = "asp-${var.base_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "P1v2"

  tags = var.tags
}

resource "azurerm_user_assigned_identity" "func_user" {
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  name                = "func-identity-${var.base_name}-${var.environment}"

  tags = var.tags
}

resource "azurerm_linux_function_app" "func_linux" {
  name                        = "func-linux-${var.base_name}-${var.environment}"
  resource_group_name         = azurerm_resource_group.rg.name
  location                    = azurerm_resource_group.rg.location
  storage_account_name        = azurerm_storage_account.apps_storage.name
  storage_account_access_key  = azurerm_storage_account.apps_storage.primary_access_key
  service_plan_id             = azurerm_service_plan.func_apps.id

  identity {
    type = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.func_user.id]
  }

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }
  }
  
  https_only = true

  tags = var.tags
}

resource "azurerm_linux_function_app_slot" "staging" {
  name                        = "staging"
  function_app_id             = azurerm_linux_function_app.func_linux.id
  storage_account_name        = azurerm_storage_account.apps_storage.name
  storage_account_access_key  = azurerm_storage_account.apps_storage.primary_access_key

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }
  }

  https_only = true

  tags = var.tags
}

data "azurerm_function_app_host_keys" "admin_key" {
  name                = azurerm_linux_function_app.func_linux.name
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_key_vault_secret" "admin_key" {
  name = "admin-api-key"
  value = data.azurerm_function_app_host_keys.admin_key.value
  key_vault_id = azurerm_key_vault.kv.id
}