namespace Client

open Fable.Import
open Thoth.Json

module LocalStorage =
  let load (decoder: Decode.Decoder<'T>) key: Result<'T,string> =
      let o = Browser.localStorage.getItem key :?> string
      if isNull o then
          "No item found in local storage with key " + key |> Error
      else
          Decode.fromString decoder o

  let delete key =
      Browser.localStorage.removeItem(key)

  let inline save key (data: 'T) =
      Browser.localStorage.setItem(key, Encode.Auto.toString(0, data))
