module Client.FileUrlHandler

open Domain.SharedTypes
open Services

type FileUrl = string

let private getUrlDbId dbId =
  dbId
  |> Dtos.getDbId
  |> sprintf "/api/thumbnails/%i"

let private unwrapDbId (dbId: DbId option) =
  match dbId with
  | Some dbId -> getUrlDbId dbId
  | None -> getUrlDbId (DbId 0)

let getUrl (imageId: ImageId): FileUrl =
  imageId
  |> Dtos.getImageId
  |> unwrapDbId
  // "https://assets.catawiki.nl/assets/2016/11/14/8/d/7/8d7a1eac-aa95-11e6-8a9e-0a43aa7714af.jpg"