resource "azurerm_app_configuration" "appconf" {
  name                = "appconf-${var.base_name}-${var.environment}-${format("%02s",var.base_instance)}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
}