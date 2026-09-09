variable "aws_region" {
  description = "AWS region for all resources."
  type        = string
}

variable "project_name" {
  description = "Project tag/name prefix for resources."
  type        = string
}

variable "environment" {
  description = "Deployment environment (e.g. dev, test, prod)."
  type        = string
}

variable "dynamodb_table_name" {
  description = "DynamoDB table name for visit write model."
  type        = string
}

variable "opensearch_domain_name" {
  description = "OpenSearch domain name for visit query model."
  type        = string
}

variable "opensearch_instance_type" {
  description = "OpenSearch data node instance type."
  type        = string
  default     = "t3.small.search"
}

variable "opensearch_instance_count" {
  description = "Number of OpenSearch data nodes."
  type        = number
  default     = 1
}
