terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.116.0"
    }
  }

  backend "azurerm" {
    resource_group_name   = "Needed-Resource"
    storage_account_name  = "requiredresources"
    container_name        = "terraform-state"
    key                   = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}
