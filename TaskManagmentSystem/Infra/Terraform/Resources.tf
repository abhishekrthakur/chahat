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
  name                = "TaskManagmenttest77"
  resource_group_name = azurerm_resource_group.FirstRG.name
  location            = azurerm_service_plan.taskmanagmentplan.location
  service_plan_id     = azurerm_service_plan.taskmanagmentplan.id

  site_config {
    always_on = false
  }
}

data "azurerm_web_app_auth_settings_v2" "auth" {
  name                = azurerm_windows_web_app.TaskManagment.name
  resource_group_name = azurerm_windows_web_app.TaskManagment.resource_group_name

  depends_on = [azurerm_windows_web_app.TaskManagment]
}