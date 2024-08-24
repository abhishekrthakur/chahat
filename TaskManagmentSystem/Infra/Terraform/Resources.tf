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

resource "azurerm_mssql_server" "tm_db_server" {
  name                         = "taskmanagmentdbservervk"
  resource_group_name          = azurerm_resource_group.FirstRG.name
  location                     = "Cental India"
  version                      = "12.0"
  administrator_login          = "adminlogin"
  administrator_login_password = "Admin@1234567"
  minimum_tls_version          = "1.2"

  tags = {
    env = "dev"
    source = "terraform"
  }
}

resource "azurerm_mssql_firewall_rule" "tm_db_server_rule" {
  name             = "firewallrule"
  server_id        = azurerm_mssql_server.tm_db_server.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_database" "tm_db" {
  name           = "taskmanagmentdb"
  server_id      = azurerm_mssql_server.tm_db_server.id
  max_size_gb    = 4
  read_scale     = false
  sku_name       = "GP_S_Gen5_2"
  zone_redundant = false
  min_capacity = 1
  auto_pause_delay_in_minutes = 4
  tags = {
    env = "dev"
    source = "terraform"
  }
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_app_configuration" "appconf" {
  name                = "appConfiguration"
  resource_group_name = azurerm_resource_group.FirstRG.name
  location            = azurerm_resource_group.FirstRG.location
}

resource "azurerm_app_configuration_key" "dbconnectionstring" {
  configuration_store_id = azurerm_app_configuration.appconf.id
  key                    = "dbconnectionstring"
  value                  = ""
}