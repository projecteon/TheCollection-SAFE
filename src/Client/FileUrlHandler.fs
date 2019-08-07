module Client.FileUrlHandler

open Domain.SharedTypes

type FileUrl = string

let getThumbnailUrl (dbId: DbId) =
  dbId.Int
  |> sprintf "/api/thumbnails/%i"

let getImageUrl (dbId: DbId) =
  dbId.Int
  |> sprintf "/api/images/%i"

let getUrl (imageId: ImageId): FileUrl =
  match imageId.Option with
  | Some dbId -> getImageUrl dbId
  | None -> getImageUrl (DbId 0)
