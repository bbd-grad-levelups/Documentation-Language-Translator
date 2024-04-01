terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
    }
  }
}

# AWS Provider Configuration
provider "aws" {
  region     = "eu-west-1"
  access_key = var.access_key #get from the IAM
  secret_key = var.secret_key #get from the IAM
}


# Create Default VPC
resource "aws_default_vpc" "doc_translator_vpc" {

  tags = {
    Name = "doc translator vpc"
  }
}

Define Subnets
data "aws_availability_zones" "available_zones" {}

resource "aws_default_subnet" "subnet_doc_translator_a" {
  availability_zone = data.aws_availability_zones.available_zones.names[0]
}

resource "aws_default_subnet" "subnet_doc_translator_b" {
  availability_zone = data.aws_availability_zones.available_zones.names[1]
}

# Define Security Group
resource "aws_security_group" "doc_translator_security_group" {
  name        = "database security group"
  description = "enable SQL Server access on port 1433"
  vpc_id      = aws_default_vpc.doc_translator_vpc.id

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "doc_translator_security_group"
  }
}

# Define DB Subnet Group
resource "aws_db_subnet_group" "doc_translator_subnet_group" {
  name       = "doc-translator-subnetgroup"
  subnet_ids = [aws_default_subnet.subnet_doc_translator_a.id, aws_default_subnet.subnet_doc_translator_b.id]

  tags = {
    Name = "doc_translator_subnet_group"
  }
}



# create the rds instance
resource "aws_db_instance" "doc_translator_db_instance" {
  engine                 = "sqlserver"
  multi_az               = false
  identifier             = "doc-translator-db"
  username               = var.db_username #use this to connect to db with SQL Server
  password               = var.db_password #use this to connect to db with SQL Server 
  instance_class         = "db.t3.small"
  allocated_storage      = 20
  publicly_accessible    = true
  db_subnet_group_name   = aws_db_subnet_group.doc_translator_subnet_group.name
  vpc_security_group_ids = [aws_security_group.doc_translator_security_group.id]
  availability_zone      = data.aws_availability_zones.available_zones.names[0]
  skip_final_snapshot    = true
}

# Define Elastic Beanstalk Application
resource "aws_beanstalk_application" "doc_translator_app" {
  name = "doc-translator-app"
}									  

# Create Elastic Beanstalk Environment
resource "aws_elastic_beanstalk_environment" "doc_translator_environment" {
  name                = "doc-translator-environment"
  application         = aws_beanstalk_application.doc_translator_app.name
  solution_stack_name = "64bit Windows Server 2019 v3.1.0 running IIS 10.0" # ASP.NET solution stack

  setting {
    namespace = "aws:ec2:vpc"
    name      = "VPCId"
    value     = aws_default_vpc.doc_translator_vpc.id
  }

  setting {
    namespace = "aws:ec2:vpc"
    name      = "Subnets"
    value     = join(",", [aws_default_subnet.subnet_beanEnthusiasts_a.id, aws_default_subnet.subnet_beanEnthusiasts_b.id])
  }
  
    setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "ASPNETCORE_ENVIRONMENT"
    value     = "Production" # ASP.NET Core environment setting (optional)
  }

}

