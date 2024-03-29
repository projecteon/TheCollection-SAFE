module ServerHost

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.StaticFiles
open Saturn

open Server.Router

let configureApp (app: IApplicationBuilder) =
  let provider = FileExtensionContentTypeProvider();
  provider.Mappings.[".webmanifest"] <- "application/manifest+json";
  app.UseStaticFiles(StaticFileOptions (ContentTypeProvider = provider));


let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app = application {
  pipe_through endpointPipe
  app_config configureApp
  url ("http://0.0.0.0:" + Server.Config.Port.ToString() + "/")
  use_router appRouter
  memory_cache
  use_static Server.Config.PublicPath
  use_gzip
  use_jwt_authentication Server.Config.JwtSecret Server.Config.JwtIssuer
  use_json_serializer (Thoth.Json.Giraffe.ThothSerializer())
}

run app

// https://dev.to/samueleresca/build-web-service-using-f-and-aspnet-core-52l8
// https://vtquan.github.io/fsharp/creating-api-with-giraffe/

