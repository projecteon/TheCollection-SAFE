namespace Server

module WebServer =
  open Giraffe

  open TeaCollection.Infrastructure.MsSql
  open Security.Authorization
  open TeaCollection.Infrastructure.MsSql.AzureStorage
  open TeaCollection.Infrastructure.MsSql.DbContext

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

  let DevDbCondig = {
    PageSize = 100 |> int64 |> PageSize
    Default = DbContext.DevConnectionString |> ConnectionString
    ReadOnly = DbContext.DevConnectionString |> ConnectionString
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
  let getBlobByFilename = AzureBlobRepository.getAsync2 azureStorageCfg AzureBlobRepository.ImagesContainerReferance

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
  let insertBlob = AzureBlobRepository.insertAsync2 azureStorageCfg AzureBlobRepository.ImagesContainerReferance

  let validate (model: 'a) =
    Domain.SharedTypes.Result.Success model

  // https://blogs.msdn.microsoft.com/dotnet/2017/09/26/build-a-web-service-with-f-and-net-core-2-0/

  let webApp: HttpHandler =
    choose [
      subRoute "/api"
        (choose [
          GET >=> routef "/thumbnails/%i" (Api.FileUpload.thumbnailHandler getFileById getBlobByFilename)
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
            route "/teabags/upload" >=> (Api.FileUpload.fileUploadHandler insertFile insertBlob)
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
