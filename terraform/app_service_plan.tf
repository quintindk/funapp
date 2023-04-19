#### APP SERVICE PLAN
resource "azurerm_storage_account" "apps-storage" {
  name                     = "stoapps${var.base_name}stg"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = var.tags
}

resource "azurerm_service_plan" "func-apps" {
  name                = "asp-${var.base_name}-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "P1v2"

  tags = var.tags
}

resource "azurerm_user_assigned_identity" "func-user" {
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  name                = "linux-func-identity-${var.base_name}-${var.environment}"

  tags = var.tags
}