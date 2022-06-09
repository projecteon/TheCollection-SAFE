module Azure

open Farmer
open Farmer.Builders
open Fake.Core
open System.Text.Json
open System

type Credentials = {
  appId: string
  displayName: string
  password: string
  tenant: string
}

//let asGitHubAction = function | Ok _ -> 0 | Error e -> (printf "%s" e) ; 1

let credentials = Environment.environVarOrFail "credentials" |> JsonSerializer.Deserialize<Credentials>
let environment = Environment.environVarOrFail "environment" |> function x -> match x with | "production" -> "" | _ -> x
let projectName  = "collection"
let prefix = sprintf "the-%s%s" projectName (if environment = "" then environment else sprintf "-%s" environment)

let insightsName = $"{prefix}-insights"
let appinsights = appInsights {
    name insightsName
}

let storageName = $"the{projectName}{environment}storage"
let storage = storageAccount {
    name storageName
    sku Storage.Sku.Standard_LRS
}

let sqlServerName = $"{prefix}-sql-server"
let dbName = $"{prefix}"
let dbUsername = $"SA-{sqlServerName}"
let dbPassword () = Environment.environVarOrFail "dbPassword"
let sqlServer = sqlServer {
    name sqlServerName
    admin_username dbUsername
    enable_azure_firewall

    add_databases [
        sqlDb {
            name dbName
            sku Sql.DtuSku.S1
            db_size (1024<Mb> * 1)
            collation "SQL_Latin1_General_CP1_CI_AS"
        }
    ]
}

let artifactDir = $"deploy"
let webAppName = $"{prefix}"
let jwt_secret () = Environment.environVarOrFail "JwtSecret"
let webApp = webApp {
    name webAppName
    //use_keyvault
    sku WebApp.Sku.F1
    link_to_app_insights appinsights
    secret_setting "jwt_secret"
    setting "jwt_issuer" "thecollection.net"
    setting "public_path" ".\public"
    setting "STORAGE_CONNECTIONSTRING" storage.Key
    connection_string  ("DB_CONNECTIONSTRING", (sqlServer.ConnectionString dbName))
    zip_deploy artifactDir
    depends_on sqlServer
    depends_on storage
    depends_on appinsights
}

let deployment = arm {
    location Location.WestEurope
    add_resource webApp
    add_resource appinsights
    add_resource storage
    add_resource sqlServer
}

let executeDeployment =
    // https://compositionalit.github.io/farmer/deployment-guidance/
    // https://octopus.com/blog/farmer-and-octopus-deploy
    // https://www.azurefromthetrenches.com/azure-sql-database-deployment-with-farmer-dbup-and-github-actions/
    printf "Authenticating with Azure\n" 
    Deploy.authenticate credentials.appId credentials.password credentials.tenant
    |> ignore

    //Environment.environVarOrFail "azuresub"
    //|> Guid.Parse
    //|> Deploy.setSubscription
    //|> ignore

    printf "Deploying Azure WebApp %s (%s) into %s using Farmer\n" webAppName prefix (deployment.Location.ToString())
    deployment
    |> Deploy.execute prefix [ (sqlServer.PasswordParameter, dbPassword()); ("jwt_secret", jwt_secret()) ]
    |> ignore
    printf "Deployment of resources into %s complete!\n" prefix
