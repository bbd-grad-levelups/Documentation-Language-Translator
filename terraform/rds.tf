module "rds" {
  source  = "terraform-aws-modules/rds/aws"
  version = "6.5.2"

  identifier = "doc-translator-db"

  family               = "sqlserver-ex-16.0" # DB parameter group
  major_engine_version = "16.00"             # DB option group
  engine               = "sqlserver-ex"
  engine_version       = "16.00"
  storage_encrypted    = false

  instance_class      = "db.t3.micro"
  create_db_instance  = true
  allocated_storage   = 20
  deletion_protection = false
  skip_final_snapshot = true

  db_subnet_group_name   = module.vpc.database_subnet_group_name
  vpc_security_group_ids = [aws_security_group.rds.id]
  publicly_accessible    = true

  username                    = "dbadmin"
  port                        = "1433"
  manage_master_user_password = true
}