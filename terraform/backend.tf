terraform {
  backend "s3" {
    bucket         = "363615071302-state"
    key            = "state/terraform.tfstate"
    region         = "eu-west-1"
    dynamodb_table = "363615071302-state"
  }
}
