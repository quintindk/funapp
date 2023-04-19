variable "location" {
  type = string
  default = "westeurope"
}
variable "base_name" {
  type = string
}
variable "environment" {
  type = string
}
variable "base_instance" {
  type = number
  default = 1
}
variable "sql_username" {
  type = string
  default = "sqldatabaseadmin"
}
variable "tags" {
  type = map(string)
  default = {}
}
variable "sql_ad_admin" {
  type = string
}