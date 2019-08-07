open System.IO
open Microsoft.AspNetCore
open Saturn

open Security.JsonWebToken
open Server.WebServer

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let app = application {
  url ("http://0.0.0.0:" + port.ToString() + "/")
  use_router webApp
  memory_cache
  use_static publicPath
  use_gzip
  use_jwt_authentication secret "thecollection.net"
  use_json_serializer (Thoth.Json.Giraffe.ThothSerializer())
}

run app

// https://dev.to/samueleresca/build-web-service-using-f-and-aspnet-core-52l8
// https://vtquan.github.io/fsharp/creating-api-with-giraffe/