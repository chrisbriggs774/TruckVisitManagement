locals {
  aws_region         = "eu-central-1"
  project_name       = "truck-visit-management"
  state_bucket       = "truck-visit-management-opentofu-state"
  lock_table         = "truck-visit-management-opentofu-locks"
  state_bucket_region = "eu-central-1"
}

remote_state {
  backend = "s3"

  config = {
    bucket         = local.state_bucket
    key            = "${path_relative_to_include()}/tofu.tfstate"
    region         = local.state_bucket_region
    encrypt        = true
    dynamodb_table = local.lock_table
  }

  generate = {
    path      = "backend.tf"
    if_exists = "overwrite_terragrunt"
  }
}
