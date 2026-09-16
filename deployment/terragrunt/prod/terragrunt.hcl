include "root" {
  path = find_in_parent_folders()
}

terraform {
  source = "../../terraform"
}

inputs = {
  aws_region                         = "eu-west-1"
  project_name                       = "truck-visit-management"
  environment                        = "prod"
  dynamodb_table_name                = "truck-visit-management-prod-visits"
  opensearch_domain_name             = "truck-visit-management-prod"
  opensearch_instance_type           = "m6g.large.search"
  opensearch_instance_count          = 2
  projection_lambda_function_name    = "truck-visit-management-prod-projection"
  projection_index_name              = "visits"
  projection_lambda_package_s3_bucket = "replace-with-deployment-artifacts-bucket"
  projection_lambda_package_s3_key   = "projection-lambda/prod/projection.zip"
}
