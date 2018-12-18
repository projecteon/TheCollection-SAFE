open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Services.Dtos

open TeaCollection.Infrastructure.MsSql

open Giraffe.Serialization

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getInitCounter() : Task<Counter> = task { return 42 }

let handleGetCounter =
  fun next ctx ->
    task {
        let! counter = getInitCounter()
        return! Successful.OK counter next ctx
    }

let searchAllTeabags = TeabagRepository.searchAll DbContext.ConnectionString
let getByIdTeabags = TeabagRepository.getById DbContext.ConnectionString

// https://blogs.msdn.microsoft.com/dotnet/2017/09/26/build-a-web-service-with-f-and-net-core-2-0/

let readImageFile imageId =
  imageId
  |> sprintf "C:\\src\\Theedatabase\\Afbeeldingen Zakjes\\%i.jpg"
  |> File.ReadAllBytes

let thumbnailHandler imageId : HttpHandler =
  fun (next : HttpFunc) (ctx : Http.HttpContext) ->
    task {
      let bytes = readImageFile imageId
      return! ctx.WriteBytesAsync bytes
    }

let webApp =
  choose [
    subRoute "/api"
      (choose [
        GET >=> choose [
          routef "/thumbnails/%i" thumbnailHandler
          route "/counter" >=> handleGetCounter
          route "/teabags" >=> (API.Generic.handleGetAll3 searchAllTeabags)
          // route "/teabags" >=> (API.Generic.handleGetAll Repository.Teabags.getAll)
          routef "/teabags/%i" (API.Generic.handleGet2 getByIdTeabags)
          route "/brands" >=> (API.Generic.handleGetAll Repository.Brands.getAll)
          route "/bagtypes" >=> (API.Generic.handleGetAll Repository.Bagtype.getAll)
          route "/country" >=> (API.Generic.handleGetAll Repository.Country.getAll)
        ]
      ])
    setStatusCode 404 >=> text "Not Found"
  ]

let configureSerialization (services:IServiceCollection) =
  let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
  fableJsonSettings.Converters.Add(Fable.JsonConverter())
  services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings)

let app = application {
  url ("http://0.0.0.0:" + port.ToString() + "/")
  use_router webApp
  memory_cache
  use_static publicPath
  service_config configureSerialization
  use_gzip
}

run app
