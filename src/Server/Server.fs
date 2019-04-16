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
let getAllRefValues = RefValueRepository.getAll DbContext.ConnectionString
let getUserByEmail = UserRepository.getByEmail DbContext.ConnectionString

let insertTeabag = TeabagRepository.insert DbContext.ConnectionString
let updateTeabag = TeabagRepository.update DbContext.ConnectionString
let insertBagtype = BagtypeRepository.insert DbContext.ConnectionString
let updateBagtype = BagtypeRepository.update DbContext.ConnectionString
let insertBrand = BrandRepository.insert DbContext.ConnectionString
let updateBrand = BrandRepository.update DbContext.ConnectionString
let insertCountry = CountryRepository.insert DbContext.ConnectionString
let updateCountry = CountryRepository.update DbContext.ConnectionString


let validate (model: 'a) =
  Domain.SharedTypes.Result.Success model

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

let fileUploadHandler =
  fun (next : HttpFunc) (ctx : Http.HttpContext) ->
    task {
      let formFeature = ctx.Features.Get<Http.Features.IFormFeature>()
      let! form = formFeature.ReadFormAsync System.Threading.CancellationToken.None
      let! result = form.Files |> Api.FileUpload.uploadFiles <| FileRepository.insert DbContext.ConnectionString
      match result with
      | Domain.SharedTypes.Result.Success x -> return! (Successful.OK (Domain.SharedTypes.ImageId x)) next ctx
      | Domain.SharedTypes.Result.Failure y -> return! (RequestErrors.BAD_REQUEST y) next ctx
    }

let webApp =
  choose [
    subRoute "/api"
      (choose [
        GET >=> routef "/thumbnails/%i" thumbnailHandler
        GET >=> authorize >=> choose [
          route "/teabags" >=> (API.Generic.handleGetAllWithPaging searchAllTeabags Transformers.transformTeabags)
          routef "/teabags/%i" (API.Generic.handleGet getByIdTeabags Transformers.transformTeabag)
          route "/teabags/countby/brands" >=> (API.Generic.handleGetAll (TeabagRepository.brandCount DbContext.ConnectionString))
          route "/teabags/countby/bagtypes" >=> (API.Generic.handleGetAll (TeabagRepository.bagtypeCount DbContext.ConnectionString))
          route "/teabags/countby/inserteddate" >=> API.Generic.handleGetTransformAll getCountByInserted Transformers.transform
          route "/brands" >=> (API.Generic.handleGetAllSearch getAllBrands)
          routef "/brands/%i" (API.Generic.handleGet getByIdBrands Transformers.transformBrand)
          route "/bagtypes" >=> (API.Generic.handleGetAllSearch getAllBagtypes)
          routef "/bagtypes/%i" (API.Generic.handleGet getByIdBagtypes Transformers.transformBagtype)
          route "/country" >=> (API.Generic.handleGetAllSearch getAllCountries)
          routef "/country/%i" (API.Generic.handleGet getByIdCountries Transformers.transformCountry)
          route "/refvalues" >=> (API.Generic.handleGetAllQuery getAllRefValues)
        ]
        POST >=> route "/token" >=> (handlePostToken getUserByEmail)
        POST >=> authorize >=> choose [
          route "/teabags/upload" >=> fileUploadHandler
          route "/teabags" >=> (API.Generic.handlePost insertTeabag Transformers.transformDtoToInsertTeabag validate)
          route "/bagtypes" >=> (API.Generic.handlePost insertBagtype Transformers.transformDtoToInsertBagtype validate)
          route "/brands" >=> (API.Generic.handlePost insertBrand Transformers.transformDtoToInsertBrand validate)
          route "/country" >=> (API.Generic.handlePost insertCountry Transformers.transformDtoToInsertCountry validate)
        ]
        PUT >=> authorize >=> choose [
          routef "/teabags/%i" (API.Generic.handlePut getByIdTeabags updateTeabag Transformers.transformDtoToUpdateTeabag validate)
          routef "/bagtypes/%i" (API.Generic.handlePut getByIdBagtypes updateBagtype Transformers.transformDtoToUpdateBagtype validate)
          routef "/brands/%i" (API.Generic.handlePut getByIdBrands updateBrand Transformers.transformDtoToUpdateBrand validate)
          routef "/country/%i" (API.Generic.handlePut getByIdCountries updateCountry Transformers.transformDtoToUpdateCountry validate)
        ]
      ])
    GET >=> routex "(/*)" >=> redirectTo false "/"
    RequestErrors.NOT_FOUND "Not found"
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
  use_jwt_authentication secret "thecollection.net"
}

run app

// https://dev.to/samueleresca/build-web-service-using-f-and-aspnet-core-52l8
// https://vtquan.github.io/fsharp/creating-api-with-giraffe/