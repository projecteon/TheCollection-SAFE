namespace Server

module WebServer =
  open System.IO
  open Microsoft.AspNetCore
  open FSharp.Control.Tasks.V2
  open Giraffe

  open TeaCollection.Infrastructure.MsSql
  open Security.Authorization

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

  let updateUser = UserRepository.update DbContext.ConnectionString
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

  let webApp: HttpHandler =
    choose [
      subRoute "/api"
        (choose [
          GET >=> routef "/thumbnails/%i" thumbnailHandler
          GET >=> authorize >=> choose [
            route "/teabags" >=> (Api.Generic.handleGetAllWithPaging searchAllTeabags Transformers.transformTeabags)
            routef "/teabags/%i" (Api.Generic.handleGet getByIdTeabags Transformers.transformTeabag)
            route "/teabags/countby/brands" >=> (Api.Generic.handleGetAll (TeabagRepository.brandCount DbContext.ConnectionString))
            route "/teabags/countby/bagtypes" >=> (Api.Generic.handleGetAll (TeabagRepository.bagtypeCount DbContext.ConnectionString))
            route "/teabags/countby/inserteddate" >=> Api.Generic.handleGetTransformAll getCountByInserted Transformers.transform
            route "/brands" >=> (Api.Generic.handleGetAllSearch getAllBrands)
            routef "/brands/%i" (Api.Generic.handleGet getByIdBrands Transformers.transformBrand)
            route "/bagtypes" >=> (Api.Generic.handleGetAllSearch getAllBagtypes)
            routef "/bagtypes/%i" (Api.Generic.handleGet getByIdBagtypes Transformers.transformBagtype)
            route "/country" >=> (Api.Generic.handleGetAllSearch getAllCountries)
            routef "/country/%i" (Api.Generic.handleGet getByIdCountries Transformers.transformCountry)
            route "/refvalues" >=> (Api.Generic.handleGetAllQuery getAllRefValues)
          ]
          POST >=> route "/token" >=> (handlePostToken getUserByEmail updateUser)
          POST >=> route "/refreshtoken" >=> (handlePostRefreshToken getUserByEmail updateUser)
          POST >=> authorize >=> choose [
            route "/teabags/upload" >=> fileUploadHandler
            route "/teabags" >=> (Api.Generic.handlePost insertTeabag Transformers.transformDtoToInsertTeabag validate)
            route "/bagtypes" >=> (Api.Generic.handlePost insertBagtype Transformers.transformDtoToInsertBagtype validate)
            route "/brands" >=> (Api.Generic.handlePost insertBrand Transformers.transformDtoToInsertBrand validate)
            route "/country" >=> (Api.Generic.handlePost insertCountry Transformers.transformDtoToInsertCountry validate)
          ]
          PUT >=> authorize >=> choose [
            routef "/teabags/%i" (Api.Generic.handlePut getByIdTeabags updateTeabag Transformers.transformDtoToUpdateTeabag validate)
            routef "/bagtypes/%i" (Api.Generic.handlePut getByIdBagtypes updateBagtype Transformers.transformDtoToUpdateBagtype validate)
            routef "/brands/%i" (Api.Generic.handlePut getByIdBrands updateBrand Transformers.transformDtoToUpdateBrand validate)
            routef "/country/%i" (Api.Generic.handlePut getByIdCountries updateCountry Transformers.transformDtoToUpdateCountry validate)
          ]
        ])
      GET >=> routex "(/*)" >=> redirectTo false "/"
      RequestErrors.NOT_FOUND "Not found"
    ]
