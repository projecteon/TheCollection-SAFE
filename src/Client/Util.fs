namespace Client

open Elmish
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Services.Dtos

module Util =
  let inline httpPost2<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
      let body = Encode.Auto.toString(0, data)
      let props = [
          RequestProperties.Method HttpMethod.POST
          Fetch.requestHeaders [
              if token.IsSome then yield HttpRequestHeaders.Authorization ("Bearer " + token.Value.ToString())
              yield HttpRequestHeaders.ContentType "application/json; charset=utf-8"
          ]
          RequestProperties.Body !^body
      ]

      try
          let! res = Fetch.fetch url props
          let! txt = res.text()
          return Decode.Auto.unsafeFromString<'Result> txt
      with
      | exn -> return! failwithf "Error while posting to %s. Message: %s" url exn.Message
   }


  let inline httpPost<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
      try
          let! res = Fetch.postRecord<'Data> url data [ Fetch.requestHeaders [
            if token.IsSome then yield HttpRequestHeaders.Authorization ("Bearer " + token.Value.ToString())
            yield HttpRequestHeaders.ContentType "application/json; charset=utf-8"
          ] ]
          let! txt = res.text()
          return Decode.Auto.unsafeFromString<'Result> txt
      with
      | exn -> return! failwithf "Error while posting to %s. Message: %s" url exn.Message
  }

  let tryAuthorizationRequest (cmd: (JWT -> Cmd<'Msg> )) (userData: UserData option) =
    match userData with
    | Some x -> cmd x.Token
    | _ -> Cmd.none