namespace Server

module WebServer =
  open Giraffe

  open TeaCollection.Infrastructure.MsSql
  open Security.Authorization
  open TeaCollection.Infrastructure.MsSql.DbContext

  open Microsoft.WindowsAzure.Storage

  let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
  let connectionString = tryGetEnv "DB_CONNECTIONSTRING" |> Option.defaultValue DevConnectionString |> ConnectionString
  let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse

  let DevDbCondig = {
    PageSize = 100 |> int64 |> PageSize
    Default = connectionString
    ReadOnly = connectionString
  }

  let searchAllTeabags = TeabagRepository.searchAll DevDbCondig
  let getByIdTeabags = TeabagRepository.getById DevDbCondig
  let getByIdBrands = BrandRepository.getById DevDbCondig
  let getAllBrands = BrandRepository.getAll DevDbCondig
  let getByIdBagtypes = BagtypeRepository.getById DevDbCondig
  let getAllBagtypes = BagtypeRepository.getAll DevDbCondig
  let getByIdCountries = CountryRepository.getById DevDbCondig
  let getAllCountries = CountryRepository.getAll DevDbCondig
  let getCountByInserted = (TeabagRepository.insertedCount DevDbCondig)
  let getAllRefValues = RefValueRepository.getAll DevDbCondig
  let getUserByEmail = UserRepository.getByEmail DevDbCondig
  let getFileById = FileRepository.getById DevDbCondig
  let getBlobByFilename = AzureBlobRepository.getAsync2 storageAccount AzureBlobRepository.ImagesContainerReferance
  let getThumbBlobByFilename = AzureBlobRepository.getAsync2 storageAccount AzureBlobRepository.ThumbnailsContainerReferance

  let updateUser = UserRepository.update DevDbCondig
  let insertTeabag = TeabagRepository.insert DevDbCondig
  let updateTeabag = TeabagRepository.update DevDbCondig
  let insertBagtype = BagtypeRepository.insert DevDbCondig
  let updateBagtype = BagtypeRepository.update DevDbCondig
  let insertBrand = BrandRepository.insert DevDbCondig
  let updateBrand = BrandRepository.update DevDbCondig
  let insertCountry = CountryRepository.insert DevDbCondig
  let updateCountry = CountryRepository.update DevDbCondig
  let insertFile = FileRepository.insert DevDbCondig
  let insertBlob = AzureBlobRepository.insertAsync2 storageAccount AzureBlobRepository.ImagesContainerReferance

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
            route "/teabags/countby/brands" >=> (Api.Generic.handleGetAll (TeabagRepository.brandCount DevDbCondig))
            route "/teabags/countby/bagtypes" >=> (Api.Generic.handleGetAll (TeabagRepository.bagtypeCount DevDbCondig))
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
