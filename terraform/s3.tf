resource "aws_s3_bucket" "beanstalk_release_bucket" {
  bucket        = "doc-translator-deploy-bucket"
  force_destroy = true
}

resource "aws_s3_bucket" "beanstalk_file_bucket" {
  bucket        = "doc-translator-file-storage"
  force_destroy = true
}