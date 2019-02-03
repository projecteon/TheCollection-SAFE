module Client.FileUrlHandler

open Domain.SharedTypes
open Services

type FileUrl = string

let private getUrlDbId (dbId: DbId) =
  dbId.Int
  |> sprintf "/api/thumbnails/%i"

let getUrl (imageId: ImageId): FileUrl =
  match imageId.Option with
  | Some dbId -> getUrlDbId dbId
  | None -> getUrlDbId (DbId 0)
  // "https://assets.catawiki.nl/assets/2016/11/14/8/d/7/8d7a1eac-aa95-11e6-8a9e-0a43aa7714af.jpg"