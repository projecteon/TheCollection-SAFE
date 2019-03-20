module Transformers

open NodaTime
open Domain.SharedTypes
open Services.Dtos
open Domain.Tea


let transform (data: CountBy<Instant> list): CountBy<UtcDateTimeString> list =
  data
  |> List.map (fun x -> { count = x.count; description = (NodaTimeHelper.toJsonString <| x.description) })

let transformRefValue (refvalue: Domain.Tea.RefValue option): Services.Dtos.RefValue option =
  match refvalue with
  | None -> None
  | Some x -> Some {
      id = x.id
      description = x.description
    }

let transformBrandToRefValue (brand: Brand): Services.Dtos.RefValue = {
  id = brand.id
  description = brand.name.String
 }

let transformBrandsToRefValues (brands: Brand list): Services.Dtos.RefValue list =
  brands
  |> List.map transformBrandToRefValue

let inline transformOption (x : ^T option) =
  match x with
  | Some x -> (^T : (member String : string) (x))
  | None -> ""

let transformTeabag (teabag: Domain.Tea.Teabag): Services.Dtos.Teabag = {
  id = teabag.id
  brand = { id = teabag.brand.id; description = teabag.brand.description }
  serie = transformOption teabag.serie
  flavour = teabag.flavour.String
  hallmark = transformOption teabag.hallmark
  bagtype = { id = teabag.bagtype.id; description = teabag.bagtype.description }
  country = transformRefValue teabag.country
  serialnumber = transformOption teabag.serialnumber
  imageid = teabag.imageid
}

let transformTeabags (data: Domain.Tea.Teabag list): Services.Dtos.Teabag list =
  data
  |> List.map transformTeabag