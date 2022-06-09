namespace Server.Api

open NodaTime
open Domain.SharedTypes
open Server.Api.Dtos
open Domain.Tea
open Domain.Types


module Transformers =
  let transform (data: CountBy<Instant> list): CountBy<UtcDateTimeString> list =
    data
    |> List.map (fun x -> { count = x.count; description = (NodaTimeHelper.toJsonString <| x.description) })

  let transformRefValue (refvalue: Domain.Tea.RefValue option): Domain.SharedTypes.RefValue option =
    match refvalue with
    | None -> None
    | Some x -> Some {
        id = x.id
        description = x.description
      }

  let transformDomainRefValue (refvalue: Domain.SharedTypes.RefValue option): Domain.Tea.RefValue option =
    match refvalue with
    | None -> None
    | Some x -> Some {
        id = x.id
        description = x.description
      }

  let transformBrandToRefValue (brand: Brand): RefValue = {
    id = brand.id
    description = brand.name.String
   }

  let transformBrandsToRefValues (brands: Brand list): RefValue list =
    brands
    |> List.map transformBrandToRefValue

  let transformBagtype (bagtype: Domain.Tea.Bagtype): Server.Api.Dtos.Bagtype = {
    id = bagtype.id
    name = Name bagtype.name.String
   }

  let transformDtoToInsertBagtype (bagtype: Server.Api.Dtos.Bagtype) : Domain.Tea.Bagtype =
    let created = System.DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc
    {
      id = DbId 0
      name = BagtypeName bagtype.name.String
      //created = created
    }

  let transformDtoToUpdateBagtype (current: Domain.Tea.Bagtype, bagtype: Server.Api.Dtos.Bagtype) : Domain.Tea.Bagtype =
    {current with
      name = BagtypeName bagtype.name.String
    }

  let transformBrand (brand: Domain.Tea.Brand): Server.Api.Dtos.Brand = {
    id = brand.id
    name = Name brand.name.String
   }

  let transformDtoToInsertBrand (brand: Server.Api.Dtos.Brand) : Domain.Tea.Brand =
    let created = System.DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc
    {
      id = DbId 0
      name = BrandName brand.name.String
      //created = created
    }

  let transformDtoToUpdateBrand (current: Domain.Tea.Brand, brand: Server.Api.Dtos.Brand) : Domain.Tea.Brand =
    {current with
      name = BrandName brand.name.String
    }

  let transformCountry (country: Domain.Tea.Country): Server.Api.Dtos.Country = {
    id = country.id
    name = Name country.name.String
   }

  let transformDtoToInsertCountry (country: Server.Api.Dtos.Country) : Domain.Tea.Country =
    let created = System.DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc
    {
      id = DbId 0
      name = CountryName country.name.String
      tlp_code = CountryCodeTLP ""
      name_nl = CountryName ""
    }

  let transformDtoToUpdateCountry (current: Domain.Tea.Country, country: Server.Api.Dtos.Country) : Domain.Tea.Country =
    {current with
      name = CountryName country.name.String
    }

  let inline transformOption (x : ^T option) =
    match x with
    | Some x -> (^T : (member String : string) (x))
    | None -> ""

  let transformTeabag (teabag: Domain.Tea.Teabag): Server.Api.Dtos.Teabag = {
    id = teabag.id
    brand = { id = teabag.brand.id; description = teabag.brand.description }
    serie = transformOption teabag.serie
    flavour = teabag.flavour.String
    hallmark = transformOption teabag.hallmark
    bagtype = { id = teabag.bagtype.id; description = teabag.bagtype.description }
    country = transformRefValue teabag.country
    serialnumber = transformOption teabag.serialnumber
    archiveNumber = teabag.archiveNumber
    imageid = teabag.imageid
    inserted = teabag.created.Instant.ToDateTimeUtc()
  }

  let transformTeabags (data: Domain.Tea.Teabag list): Server.Api.Dtos.Teabag list =
    data
    |> List.map transformTeabag

  let transformDtoTeabag (teabag: Server.Api.Dtos.Teabag): UpsertTeabag = {
    brand = { id = teabag.brand.id; description = teabag.brand.description }
    serie = teabag.serie |> Serie |> Some
    flavour = teabag.flavour |> Flavour
    hallmark = teabag.hallmark |> Hallmark |> Some
    bagtype = { id = teabag.bagtype.id; description = teabag.bagtype.description }
    country = teabag.country
    serialnumber = teabag.serialnumber |> SerialNumber |> Some
    imageid = teabag.imageid
  }

  let transformDtoToInsertTeabag (teabag: Server.Api.Dtos.Teabag) : Domain.Tea.Teabag =
    let created = System.DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc
    {
      id = DbId 0
      brand = { id = teabag.brand.id; description = teabag.brand.description }
      serie = teabag.serie |> Serie |> Some
      flavour = teabag.flavour |> Flavour
      hallmark = teabag.hallmark |> Hallmark |> Some
      bagtype = { id = teabag.bagtype.id; description = teabag.bagtype.description }
      country = teabag.country |> transformDomainRefValue
      serialnumber = teabag.serialnumber |> SerialNumber |> Some
      archiveNumber = teabag.archiveNumber
      imageid = teabag.imageid
      created = created |> CreatedDate
    }

  let transformDtoToUpdateTeabag (current: Domain.Tea.Teabag, teabag: Server.Api.Dtos.Teabag) : Domain.Tea.Teabag =
    {current with
      brand = { id = teabag.brand.id; description = teabag.brand.description }
      serie = teabag.serie |> Serie |> Some
      flavour = teabag.flavour |> Flavour
      hallmark = teabag.hallmark |> Hallmark |> Some
      bagtype = { id = teabag.bagtype.id; description = teabag.bagtype.description }
      country = teabag.country |> transformDomainRefValue
      serialnumber = teabag.serialnumber |> SerialNumber |> Some
      imageid = teabag.imageid
    }
