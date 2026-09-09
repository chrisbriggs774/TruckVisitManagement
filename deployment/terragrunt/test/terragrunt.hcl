include "root" {
  path = find_in_parent_folders()
}

terraform {
  source = "../../terraform"
}

inputs = {
  aws_region               = "eu-central-1"
  project_name             = "truck-visit-management"
  environment              = "test"
  dynamodb_table_name      = "truck-visit-management-test-visits"
  opensearch_domain_name   = "truck-visit-management-test"
  opensearch_instance_type = "t3.small.search"
  opensearch_instance_count = 1
}
