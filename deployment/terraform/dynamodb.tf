resource "aws_dynamodb_table" "visit_write_model" {
  name         = var.dynamodb_table_name
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "VisitId"

  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"

  attribute {
    name = "VisitId"
    type = "S"
  }

  tags = local.common_tags
}
