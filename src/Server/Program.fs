open System.IO
open Microsoft.AspNetCore
open Microsoft.Extensions.DependencyInjection
open Saturn
open Thoth.Json

open Security.JsonWebToken
open Server.WebServer

open Giraffe.Serialization

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let configureSerialization (services:IServiceCollection) =
  services.AddSingleton<IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
  url ("http://0.0.0.0:" + port.ToString() + "/")
  use_router webApp
  memory_cache
  use_static publicPath
  service_config configureSerialization
  use_gzip
  use_jwt_authentication secret "thecollection.net"
}

run app

// https://dev.to/samueleresca/build-web-service-using-f-and-aspnet-core-52l8
// https://vtquan.github.io/fsharp/creating-api-with-giraffe/