resource "azurerm_resource_group" "FirstRG" {
  name = "TaskManagmentWebApp"
  location = "southindia"
  tags = {
    env = "dev"
    source = "terraform"
  }
}

resource "azurerm_service_plan" "taskmanagmentplan" {
  name                = "taskmanagmentplan"
  resource_group_name = azurerm_resource_group.FirstRG.name
  location            = azurerm_resource_group.FirstRG.location
  sku_name            = "F1"
  os_type             = "Windows"
}

resource "azurerm_windows_web_app" "TaskManagment" {
  name                = "TaskManagment-vk"
  resource_group_name = azurerm_resource_group.FirstRG.name
  location            = azurerm_service_plan.taskmanagmentplan.location
  service_plan_id     = azurerm_service_plan.taskmanagmentplan.id

  site_config {
    application_stack {
      dotnet_version = "v8.0"
    }
    cors {
      allowed_origins = ["*"]
    }
    always_on = false
  }
}