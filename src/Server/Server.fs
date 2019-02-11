open System.IO
open Microsoft.AspNetCore
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Thoth.Json

//open Services.Dtos
open TeaCollection.Infrastructure.MsSql
open Security

open Giraffe.Serialization

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let searchAllTeabags = TeabagRepository.searchAll DbContext.ConnectionString
let getByIdTeabags = TeabagRepository.getById DbContext.ConnectionString
let getByIdBrands = BrandRepository.getById DbContext.ConnectionString
let getAllBrands = BrandRepository.getAll DbContext.ConnectionString
let getByIdBagtypes = BagtypeRepository.getById DbContext.ConnectionString
let getAllBagtypes = BagtypeRepository.getAll DbContext.ConnectionString
let getByIdCountries = CountryRepository.getById DbContext.ConnectionString
let getAllCountries = CountryRepository.getAll DbContext.ConnectionString
let getCountByInserted = (TeabagRepository.insertedCount DbContext.ConnectionString)

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
    POST >=> route "/api/token" >=> handlePostToken
    GET >=> routef "/api/thumbnails/%i" thumbnailHandler
    subRoute "/api"
      (choose [
        GET >=> authorize >=> choose [
          route "/teabags" >=> (API.Generic.handleGetAllWithPaging searchAllTeabags)
          routef "/teabags/%i" (API.Generic.handleGet getByIdTeabags)
          route "/teabags/countby/brands" >=> (API.Generic.handleGetAll (TeabagRepository.brandCount DbContext.ConnectionString))
          route "/teabags/countby/bagtypes" >=> (API.Generic.handleGetAll (TeabagRepository.bagtypeCount DbContext.ConnectionString))
          route "/teabags/countby/inserteddate" >=> API.Generic.handleGetTransformAll getCountByInserted Transformers.transform
          route "/brands" >=> (API.Generic.handleGetAllSearch getAllBrands)
          route "/bagtypes" >=> (API.Generic.handleGetAllSearch getAllBagtypes)
          route "/country" >=> (API.Generic.handleGetAllSearch getAllCountries)
        ]
      ])
    setStatusCode 404 >=> text "Not Found"
  ]

let configureSerialization (services:IServiceCollection) =
  services.AddSingleton<IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
  url ("http://0.0.0.0:" + port.ToString() + "/")
  use_router webApp
  memory_cache
  use_static publicPath
  service_config configureSerialization
  use_gzip
  use_jwt_authentication secret "jwtwebapp.net"
}

run app
