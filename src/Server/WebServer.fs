namespace Server

module WebServer =
  open Giraffe

  open TeaCollection.Infrastructure.MsSql
  open Security.Authorization
  open TeaCollection.Infrastructure.MsSql.DbContext

  let DbConfig = {
    PageSize = 100 |> int64 |> PageSize
    Default = Config.connectionString
    ReadOnly = Config.connectionString
  }

  let statistics = TeabagRepository.statistics DbConfig
  let searchAllTeabags = TeabagRepository.searchAll DbConfig
  let getByIdTeabags = TeabagRepository.getById DbConfig
  let getByIdBrands = BrandRepository.getById DbConfig
  let getAllBrands = BrandRepository.getAll DbConfig
  let getByIdBagtypes = BagtypeRepository.getById DbConfig
  let getAllBagtypes = BagtypeRepository.getAll DbConfig
  let getByIdCountries = CountryRepository.getById DbConfig
  let getAllCountries = CountryRepository.getAll DbConfig
  let getCountByInserted = (TeabagRepository.insertedCount DbConfig)
  let getAllRefValues = RefValueRepository.getAll DbConfig
  let getUserByEmail = UserRepository.getByEmail DbConfig
  let getFileById = FileRepository.getById DbConfig
  let getBlobByFilename = AzureBlobRepository.getAsync2 Config.storageAccount AzureBlobRepository.ImagesContainerReferance
  let getThumbBlobByFilename = AzureBlobRepository.getAsync2 Config.storageAccount AzureBlobRepository.ThumbnailsContainerReferance

  let updateUser = UserRepository.update DbConfig
  let insertTeabag = TeabagRepository.insert DbConfig
  let updateTeabag = TeabagRepository.update DbConfig
  let insertBagtype = BagtypeRepository.insert DbConfig
  let updateBagtype = BagtypeRepository.update DbConfig
  let insertBrand = BrandRepository.insert DbConfig
  let updateBrand = BrandRepository.update DbConfig
  let insertCountry = CountryRepository.insert DbConfig
  let updateCountry = CountryRepository.update DbConfig
  let insertFile = FileRepository.insert DbConfig
  let insertBlob = AzureBlobRepository.insertAsync2 Config.storageAccount AzureBlobRepository.ImagesContainerReferance

  let validate (model: 'a) =
    Domain.SharedTypes.Result.Success model

  // https://blogs.msdn.microsoft.com/dotnet/2017/09/26/build-a-web-service-with-f-and-net-core-2-0/

  let webApp: HttpHandler =
    choose [
      subRoute "/api"
        (choose [
          GET >=> routef "/thumbnails/%i" (Api.File.get getFileById getThumbBlobByFilename)
          GET >=> routef "/images/%i" (Api.File.get getFileById getBlobByFilename)
          GET >=> authorize >=> choose [
            route "/teabags" >=> (Api.Generic.handleGetAllWithPaging searchAllTeabags Transformers.transformTeabags)
            routef "/teabags/%i" (Api.Generic.handleGet getByIdTeabags Transformers.transformTeabag)
            route "/teabags/countby/brands" >=> (Api.Generic.handleGetAll (TeabagRepository.brandCount DbConfig))
            route "/teabags/countby/bagtypes" >=> (Api.Generic.handleGetAll (TeabagRepository.bagtypeCount DbConfig))
            route "/teabags/countby/inserteddate" >=> Api.Generic.handleGetTransformAll getCountByInserted Transformers.transform
            route "/teabags/statistics" >=> Api.Generic.handleGetAll statistics
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
            route "/teabags/upload" >=> (Api.File.upload insertFile insertBlob)
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
