terraform {
  required_version = ">= 1.5.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.39.1"
    }
  }
}

provider "aws" {
  profile = "bbd-grad"
  region = "eu-west-1"
  default_tags {
    tags = {
      "owner"         = "ryan.trickett@bbd.co.za"
      "created-using" = "terraform"
    }
  }
}