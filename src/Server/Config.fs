namespace Server
  open System.IO
  open Microsoft.WindowsAzure.Storage;
  open Microsoft.WindowsAzure.Storage.Blob;
  open TeaCollection.Infrastructure.MsSql
  open TeaCollection.Infrastructure.MsSql.DbContext

  module Config =
    // type DbConfig = {
    //   Database: DbContext.DbConfig
    //   AzureStorage: AzureStorage.StorageAccount
    // }

    let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
    let connectionString = tryGetEnv "SQLCONNSTR_DB_CONNECTIONSTRING" |> Option.defaultValue DevConnectionString |> ConnectionString
    let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse

    // https://medium.com/@dsincl12/json-web-token-with-giraffe-and-f-4cebe1c3ef3b
    // https://github.com/giraffe-fsharp/Giraffe/blob/master/samples/JwtApp/JwtApp/Program.fs
    let JwtSecret = tryGetEnv "jwt_secret" |> Option.defaultValue "spadR2dre#u-ruBrE@TepA&*Uf@U"
    let JwtIssuer = tryGetEnv "jwt_issuer" |> Option.defaultValue "thecollection.net"
    let PublicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
    let Port = 8085us
