namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Types
open Domain.Tea
open Search
open Util

module RefValueRepository =
  let mapBrand (brand: Domain.Tea.Brand): Services.Dtos.RefValue = {
    id = brand.id
    description = brand.name.String
  }

  let mapBagtype (brand: Domain.Tea.Bagtype): Services.Dtos.RefValue = {
    id = brand.id
    description = brand.name.String
  }

  let mapCountry (brand: Domain.Tea.Country): Services.Dtos.RefValue = {
    id = brand.id
    description = brand.name.String
  }

  let addWildcardSuffic (term: SearchTerm) =
    match term with
    | Some searchTerm -> 
      if searchTerm.EndsWith("%") then searchTerm |> Some
      else sprintf "%s%%" searchTerm |> Some;
    | None -> "%%" |> Some

  let getAll (connectiongString: string) (searchParams: RefValueSearchParams) : Task<Services.Dtos.RefValue list> =
      task {
        let searchFilter: SearchParams = {Term = searchParams.Term |> addWildcardSuffic; Page = None}
        match searchParams.RefValueType with
        | RefValueTypes.Brand ->
          let! brands = (BrandRepository.getAll connectiongString searchFilter)
          return brands |> List.map mapBrand
        | RefValueTypes.Bagtype ->
          let! brands = (BagtypeRepository.getAll connectiongString searchFilter)
          return brands |> List.map mapBagtype
        | RefValueTypes.Country ->
          let! brands = (CountryRepository.getAll connectiongString searchFilter)
          return brands |> List.map mapCountry
      }