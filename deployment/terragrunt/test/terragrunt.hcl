include "root" {
  path = find_in_parent_folders()
}

terraform {
  source = "../../terraform"
}

inputs = {
  aws_region                         = "eu-west-1"
  project_name                       = "truck-visit-management"
  environment                        = "test"
  dynamodb_table_name                = "truck-visit-management-test-visits"
  opensearch_domain_name             = "truck-visit-management-test"
  opensearch_instance_type           = "t3.small.search"
  opensearch_instance_count          = 1
  projection_lambda_function_name    = "truck-visit-management-test-projection"
  projection_index_name              = "visits"
  projection_lambda_package_s3_bucket = "replace-with-deployment-artifacts-bucket"
  projection_lambda_package_s3_key   = "projection-lambda/test/projection.zip"
}
