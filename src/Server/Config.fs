namespace Server
  open System.IO
  open Microsoft.WindowsAzure.Storage;
  open Microsoft.WindowsAzure.Storage.Blob;
  open Infrastructure.Data
  open Infrastructure.Data.DbContext

  module TryParser =
    // convenient, functional TryParse wrappers returning option<'a>
    let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
        | true, v    -> Some v
        | false, _   -> None

    let tryParseOPtionWith (tryParseFunc: string -> bool * _) (value: string option) =
      match value with
      | Some x    -> (tryParseWith tryParseFunc) x
      | None      -> None

    let parseDate   = tryParseWith System.DateTime.TryParse
    let parseInt    = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    // etc.

    let parseIntOption = tryParseOPtionWith System.Int32.TryParse

    // active patterns for try-parsing strings
    let (|Date|_|)   = parseDate
    let (|Int|_|)    = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble

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

    let pageSize = tryGetEnv "api_pagesize" |> TryParser.parseIntOption  |> Option.defaultValue 100
    let DbConfig = {
      PageSize = pageSize |> int64 |> PageSize
      Default = connectionString
      ReadOnly = connectionString
    }
