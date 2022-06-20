namespace Server

module Router =
  open Giraffe
  open Saturn
  open System.Threading.Tasks
  open FSharp.Control.Tasks.V2

  open Infrastructure.Data
  open Security.Authorization
  open Infrastructure.Data.DbContext
  open Server.Api

  let validate (model: 'a) =
    Domain.SharedTypes.Result.Success model

  let searchAllTeabags = TeabagRepository.searchAll Config.DbConfig
  let getByIdTeabags = TeabagRepository.getById Config.DbConfig
  let insertTeabag = TeabagRepository.insert Config.DbConfig
  let updateTeabag = TeabagRepository.update Config.DbConfig

  let statistics = TeabagRepository.statistics Config.DbConfig
  let getCountByInserted = TeabagRepository.insertedCount Config.DbConfig

  let teabagsRouter = router {
    pipe_through authorize
    get "" (Api.Generic.handleGetAllWithPaging searchAllTeabags Transformers.transformTeabags)
    get "/" (Api.Generic.handleGetAllWithPaging searchAllTeabags Transformers.transformTeabags)
    getf "/%i" (Api.Generic.handleGet getByIdTeabags Transformers.transformTeabag)
    post "" (Api.Generic.handlePost insertTeabag Transformers.transformDtoToInsertTeabag validate)
    post "/" (Api.Generic.handlePost insertTeabag Transformers.transformDtoToInsertTeabag validate)
    putf "/%i" (Api.Generic.handlePut getByIdTeabags updateTeabag Transformers.transformDtoToUpdateTeabag validate)

    get "/statistics" (Api.Generic.handleGetAll statistics)
    get "/countby/countrytlp" (Api.Generic.handleGetAll (TeabagRepository.countryTLDCount Config.DbConfig))
    get "/countby/brands" (Api.Generic.handleGetAll (TeabagRepository.brandCount Config.DbConfig))
    get "/countby/bagtypes" (Api.Generic.handleGetAll (TeabagRepository.bagtypeCount Config.DbConfig))
    get "/countby/inserteddate" (Api.Generic.handleGetTransformAll getCountByInserted Transformers.transform)
  }

  let getAllBrands = BrandRepository.getAll Config.DbConfig
  let getByIdBrands = BrandRepository.getById Config.DbConfig
  let insertBrand = BrandRepository.insert Config.DbConfig
  let updateBrand = BrandRepository.update Config.DbConfig
  let brandsRouter = router {
    pipe_through authorize
    get "" (Api.Generic.handleGetAllSearch getAllBrands)
    get "/" (Api.Generic.handleGetAllSearch getAllBrands)
    getf "/%i" (Api.Generic.handleGet getByIdBrands Transformers.transformBrand)
    post "/" (Api.Generic.handlePost insertBrand Transformers.transformDtoToInsertBrand validate)
    putf "/%i" (Api.Generic.handlePut getByIdBrands updateBrand Transformers.transformDtoToUpdateBrand validate)
  }


  let getAllBagtypes = BagtypeRepository.getAll Config.DbConfig
  let getByIdBagtypes = BagtypeRepository.getById Config.DbConfig
  let insertBagtype = BagtypeRepository.insert Config.DbConfig
  let updateBagtype = BagtypeRepository.update Config.DbConfig
  let bagtypesRouter = router {
    pipe_through authorize
    get "" (Api.Generic.handleGetAllSearch getAllBagtypes)
    get "/" (Api.Generic.handleGetAllSearch getAllBagtypes)
    getf "/%i" (Api.Generic.handleGet getByIdBagtypes Transformers.transformBagtype)
    post "/" (Api.Generic.handlePost insertBagtype Transformers.transformDtoToInsertBagtype validate)
    putf "/%i" (Api.Generic.handlePut getByIdBagtypes updateBagtype Transformers.transformDtoToUpdateBagtype validate)
  }

  let getAllCountries = CountryRepository.getAll Config.DbConfig
  let getByIdCountries = CountryRepository.getById Config.DbConfig
  let insertCountry = CountryRepository.insert Config.DbConfig
  let updateCountry = CountryRepository.update Config.DbConfig
  let countryRouter = router {
    pipe_through authorize
    get "" (Api.Generic.handleGetAllSearch getAllCountries)
    get "/" (Api.Generic.handleGetAllSearch getAllCountries)
    getf "/%i" (Api.Generic.handleGet getByIdCountries Transformers.transformCountry)
    post "/" (Api.Generic.handlePost insertCountry Transformers.transformDtoToInsertCountry validate)
    putf "/%i" (Api.Generic.handlePut getByIdCountries updateCountry Transformers.transformDtoToUpdateCountry validate)
  }

  let getAllRefValues = RefValueRepository.getAll Config.DbConfig
  let refvaluesRouter = router {
    pipe_through authorize
    get "" (Api.Generic.handleGetAllQuery getAllRefValues)
    get "/" (Api.Generic.handleGetAllQuery getAllRefValues)
  }

  let api = pipeline {
    plug acceptJson
    set_header "x-pipeline-type" "Api"
  }

  let getUserByEmail = UserRepository.getByEmail Config.DbConfig
  let updateUser = UserRepository.update Config.DbConfig
  let apiRouter = router {
    not_found_handler (setStatusCode 404 >=> text "Api 404")
    pipe_through api

    post "/token" (handlePostToken getUserByEmail updateUser)
    post "/refreshtoken" (handlePostRefreshToken getUserByEmail updateUser)

    forward "/refvalues" refvaluesRouter
    forward "/teabags" teabagsRouter
    forward "/brands" brandsRouter
    forward "/bagtypes" brandsRouter
    forward "/country" countryRouter
  }

  let getFileById = FileRepository.getById Config.DbConfig
  let getBlobByFilename = AzureBlobRepository.getAsync2 Config.storageAccount AzureBlobRepository.ImagesContainerReferance
  let getThumbBlobByFilename = AzureBlobRepository.getAsync2 Config.storageAccount AzureBlobRepository.ThumbnailsContainerReferance
  let insertFile = FileRepository.insert Config.DbConfig
  let insertBlob = AzureBlobRepository.insertAsync2 Config.storageAccount AzureBlobRepository.ImagesContainerReferance
  let fileRouter = router {
    not_found_handler (setStatusCode 404 >=> text "File 404")

    getf "/teabags/thumbnails/%i" (Api.File.get getFileById getThumbBlobByFilename)
    getf "/teabags/images/%i" (Api.File.get getFileById getBlobByFilename)

    post "/teabags/upload" (Api.File.upload insertFile insertBlob)
  }

  let appRouter = router {
    forward "/api" apiRouter
    forward "/resource" fileRouter

    get "/*" (redirectTo false "/")
  }