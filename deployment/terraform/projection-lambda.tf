resource "aws_iam_role" "projection_lambda" {
  name = "${var.project_name}-${var.environment}-projection-lambda-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      }
    ]
  })

  tags = local.common_tags
}

resource "aws_iam_role_policy" "projection_lambda" {
  name = "${var.project_name}-${var.environment}-projection-lambda-policy"
  role = aws_iam_role.projection_lambda.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Sid    = "CloudWatchLogs"
        Effect = "Allow"
        Action = [
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents"
        ]
        Resource = "arn:aws:logs:*:*:*"
      },
      {
        Sid    = "ReadDynamoDbStream"
        Effect = "Allow"
        Action = [
          "dynamodb:DescribeStream",
          "dynamodb:GetRecords",
          "dynamodb:GetShardIterator",
          "dynamodb:ListStreams"
        ]
        Resource = aws_dynamodb_table.visit_write_model.stream_arn
      },
      {
        Sid    = "ReadDynamoDbItem"
        Effect = "Allow"
        Action = [
          "dynamodb:GetItem"
        ]
        Resource = aws_dynamodb_table.visit_write_model.arn
      },
      {
        Sid    = "WriteOpenSearch"
        Effect = "Allow"
        Action = [
          "es:ESHttpPost",
          "es:ESHttpPut",
          "es:ESHttpDelete",
          "es:ESHttpGet"
        ]
        Resource = "${aws_opensearch_domain.visit_read_model.arn}/*"
      }
    ]
  })
}

resource "aws_lambda_function" "projection" {
  function_name = var.projection_lambda_function_name
  role          = aws_iam_role.projection_lambda.arn
  runtime       = "dotnet8"
  handler       = "TruckVisitManagement.Projection.Lambda::TruckVisitManagement.Projection.Lambda.Function::FunctionHandler"
  timeout       = 30
  memory_size   = 512

  s3_bucket = var.projection_lambda_package_s3_bucket
  s3_key    = var.projection_lambda_package_s3_key

  environment {
    variables = {
      OPENSEARCH_ENDPOINT = "https://${aws_opensearch_domain.visit_read_model.endpoint}"
      OPENSEARCH_INDEX    = var.projection_index_name
      DYNAMODB_TABLE_NAME = var.dynamodb_table_name
    }
  }

  tags = local.common_tags
}

resource "aws_lambda_event_source_mapping" "projection_stream_mapping" {
  event_source_arn  = aws_dynamodb_table.visit_write_model.stream_arn
  function_name     = aws_lambda_function.projection.arn
  starting_position = "LATEST"
  batch_size        = 100
  enabled           = true
}
