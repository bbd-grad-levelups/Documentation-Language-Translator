terraform {
  backend "s3" {
    bucket         = "387198229710-state"
    key            = "state/terraform.tfstate"
    region         = "eu-west-1"
    dynamodb_table = "387198229710-state"
  }
}
