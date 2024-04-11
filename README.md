<div align="center">

# Doc#mentation Translator - Project Description

<br>
  
[![Commits](https://img.shields.io/github/commit-activity/w/bbd-grad-levelups/Documentation-Language-Translator)](https://github.com/bd-grad-levelups/Documentation-Language-Translator/activity)
[![CI/CD](https://github.com/bbd-grad-levelups/Documentation-Language-Translator/actions/workflows/proj-ci-cd.yaml/badge.svg)](https://github.com/bbd-grad-levelups/Documentation-Language-Translator/actions/workflows/proj-ci-cd.yaml)

</div>

Welcome to the new and growing cloud solution for managing your Bean Crop Production. AgriCloud provides farmers with easy access to the Bean Cloud via their Google account, empowering them to efficiently manage their bean provisions.

Create, Update, Terminate, and Grow!

With AgriCloud, farmers can effortlessly perform a variety of bean-related activities with just a single click. Adjust soil moisture levels with ease, add water as needed! Require more sunlight? No problem! AgriCloud offers real-time crop management capabilities, allowing you to optimize your farming practices and become the best farmer you can be.


## Project Resources:

[![Documentation](https://img.shields.io/badge/View-Project%20Documentation-blue?style=for-the-badge)](https://rockshopgraduate.atlassian.net/wiki/spaces/DLTC/overview)&ensp;

[![Project Management](https://img.shields.io/badge/View-Project%20Issue%20Board-blue?style=for-the-badge)](https://rockshopgraduate.atlassian.net/jira/software/projects/DLT/boards/6)&ensp;


## Setup
1. Clone this repository to your local machine. 
   ```bash
   git clone https://github.com/bbd-grad-levelups/Documentation-Language-Translator/
   ```

2. Set up a role in your AWS account for this repository. Follow the instructions provided in the AWS Security Blog: [Use IAM Roles to Connect GitHub Actions to Actions in AWS](https://aws.amazon.com/blogs/security/use-iam-roles-to-connect-github-actions-to-actions-in-aws/).

3. Configure secret variables in the GitHub repository settings:
   - Go to Settings > Secrets and Variables.
   - Under Actions, set the secret variable "AWS_ASSUME_ROLE" to the ARN of your IAM role.

4. Setup Terraform
   - Setup local AWS auth for account
   - Navigate to bootstrap folder
   - Run "terraform init" and then "terraform apply"
   - Pipeline deploys the actual app

## Runing fully locally
This project uses .NET 8.

#### API
**Found in `DocumentationTranslator/WebServer`**

The .NET application sources an enviroment variables file for secrets. The required values are as follows:
```env
DocServer_ConnectionString = 
Server=<url>;Database=<db_name>;User Id=<username>;Password=<password>;
DocServer_TranslationAPIKey = <translation_key>
DocServer_WEB_audience = <web_oauth_id>
DocServer_CLI_audience = <cli_oauth_id>
DocServer_FileBucket = <bucket_name>
```

To run this, we can use `dotnet run` to run in the terminal.

#### CLI
**Found in `DocumentationTranslator/Cli`**

The CLI retrieves its parameters from a `client_secrets.json` stored in the CLI project root. Required values as follows:
```json
{
    "installed": {
        "client_id": "client_id",
        "project_id": "proh_id",
        "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        "token_uri": "https://oauth2.googleapis.com/token",
        "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        "client_secret": "client_secret",
        "redirect_uri": "urn:ietf:wg:oauth:2.0:oob"
    }
}
```

To run this, we can use `dotnet run` to run in the terminal.

#### CLI
**Found in `DocumentationTranslator/WebApp`**

Similarly to the CLI, we store parameters in a `client_secrets.json` stored in the Web App project root. Required values as follows:
```json
{
    "installed": {
        "client_id": "client_id",
        "project_id": "proh_id",
        "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        "token_uri": "https://oauth2.googleapis.com/token",
        "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        "client_secret": "client_secret",
        "redirect_uri": "urn:ietf:wg:oauth:2.0:oob"
    }
}
```

To run this, we can use `dotnet run` to run in the terminal.

#### Database
Requires a MS SQL Server database. You can set this up however you want, and choose your own username and password, as long as you set the env variables.
