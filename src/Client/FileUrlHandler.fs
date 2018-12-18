module Client.FileUrlHandler

open Domain.Types

type FileUrl = string

let getUrl (id: ImageId): FileUrl =
  match id with
  | Some x -> sprintf "/api/thumbnails/%i" x
  | None -> "/api/thumbnails/0"
  // "https://assets.catawiki.nl/assets/2016/11/14/8/d/7/8d7a1eac-aa95-11e6-8a9e-0a43aa7714af.jpg"