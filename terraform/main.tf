terraform {
  required_providers {
    random = {
      source  = "hashicorp/random"
      version = "3.0.1"
    }
    azurerm = {
      version = "=3.52.0"
    }
    azuread = {
      source = "hashicorp/azuread"
      version = "2.37.1"
    }
  }
}

# Random provider
provider "random" {}

# Subscription provider
provider "azurerm" {
  features {}
}

# Azure Active Directory
provider "azuread" {}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "rg" {
  name = "rg-${var.base_name}-${var.environment}-${format("%0d",var.base_instance)}"
  location = var.location
}