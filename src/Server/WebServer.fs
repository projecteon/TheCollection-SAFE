namespace Server

module WebServer =
  open System.IO
  open Microsoft.AspNetCore
  open FSharp.Control.Tasks.V2
  open Giraffe

  open TeaCollection.Infrastructure.MsSql
  open Security.Authorization
  open TeaCollection.Infrastructure.MsSql.AzureStorage

  let azureStorageCfg = {
    AccountName = AccountName "devstoreaccount1"
    AccountKey = AccountKey "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="
    Scheme= Scheme "http"
    Endpoints = {
      BlobEndpoint = BlobEndpoint "http://127.0.0.1:10000/devstoreaccount1"
      TableEndpoint = TableEndpoint "http://127.0.0.1:10000/devstoreaccount1"
      QueueEndpoint = QueueEndpoint "http://127.0.0.1:10000/devstoreaccount1"
    }
  }

  let searchAllTeabags = TeabagRepository.searchAll DbContext.DevConnectionString
  let getByIdTeabags = TeabagRepository.getById DbContext.DevConnectionString
  let getByIdBrands = BrandRepository.getById DbContext.DevConnectionString
  let getAllBrands = BrandRepository.getAll DbContext.DevConnectionString
  let getByIdBagtypes = BagtypeRepository.getById DbContext.DevConnectionString
  let getAllBagtypes = BagtypeRepository.getAll DbContext.DevConnectionString
  let getByIdCountries = CountryRepository.getById DbContext.DevConnectionString
  let getAllCountries = CountryRepository.getAll DbContext.DevConnectionString
  let getCountByInserted = (TeabagRepository.insertedCount DbContext.DevConnectionString)
  let getAllRefValues = RefValueRepository.getAll DbContext.DevConnectionString
  let getUserByEmail = UserRepository.getByEmail DbContext.DevConnectionString

  let updateUser = UserRepository.update DbContext.DevConnectionString
  let insertTeabag = TeabagRepository.insert DbContext.DevConnectionString
  let updateTeabag = TeabagRepository.update DbContext.DevConnectionString
  let insertBagtype = BagtypeRepository.insert DbContext.DevConnectionString
  let updateBagtype = BagtypeRepository.update DbContext.DevConnectionString
  let insertBrand = BrandRepository.insert DbContext.DevConnectionString
  let updateBrand = BrandRepository.update DbContext.DevConnectionString
  let insertCountry = CountryRepository.insert DbContext.DevConnectionString
  let updateCountry = CountryRepository.update DbContext.DevConnectionString

  let validate (model: 'a) =
    Domain.SharedTypes.Result.Success model

  // https://blogs.msdn.microsoft.com/dotnet/2017/09/26/build-a-web-service-with-f-and-net-core-2-0/

  let thumbnailHandler imageId : HttpHandler =
    fun (next : HttpFunc) (ctx : Http.HttpContext) ->
      task {
        //let! bytes = ImageFilesystemRepository.getAsync imageId
        let! file = FileRepository.getById DbContext.DevConnectionString imageId
        match file with
        | None -> return! (RequestErrors.NOT_FOUND (sprintf "%i" imageId)) next ctx
        | Some x ->
          let! blobStorageContainer = Server.AzureBlobRepository.createContainer azureStorageCfg AzureBlobRepository.ImagesContainerReferance
          let! bytes = AzureBlobRepository.getAsync blobStorageContainer x.filename
          return! ctx.WriteBytesAsync bytes
      }

  let fileUploadHandler =
    fun (next : HttpFunc) (ctx : Http.HttpContext) ->
      task {
        let formFeature = ctx.Features.Get<Http.Features.IFormFeature>()
        let! form = formFeature.ReadFormAsync System.Threading.CancellationToken.None
        let! result = form.Files |> Api.FileUpload.uploadFiles <| FileRepository.insert DbContext.DevConnectionString
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
            route "/teabags/countby/brands" >=> (Api.Generic.handleGetAll (TeabagRepository.brandCount DbContext.DevConnectionString))
            route "/teabags/countby/bagtypes" >=> (Api.Generic.handleGetAll (TeabagRepository.bagtypeCount DbContext.DevConnectionString))
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
