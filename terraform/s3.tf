resource "aws_s3_bucket" "beanstalk_release_bucket" {
  bucket        = "doc-translator-deploy-bucket"
  force_destroy = true
}