namespace Client

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fetch
open Elmish
open Thoth.Json

open Domain.SharedTypes
open Server.Api.Dtos

// https://gist.github.com/MangelMaxime/8d3758cbc0bc69ffb9a156db294ea8e9
module Http =
  let errorString (response: Response) =
    string response.Status + " " + response.StatusText + " for URL " + response.Url

  let unauthorizedString (response: Response) = promise {
    try
      let! text = response.text()
      return text
    with
    | exn -> return (errorString response)
  }

  // https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
  type FetchResult =
    | Success of Response
    | BadStatus of Response
    | NetworkError
    | Unauthorized of Response

  let fetch (url: string) (init: RequestProperties list) : JS.Promise<FetchResult> =
    GlobalFetch.fetch(RequestInfo.Url url, requestProps init)
    |> Promise.map (fun response ->
      if response.Ok then
        Success response
      else
        if response.Status = 401 then
          Unauthorized response
        else if response.Status < 200 || response.Status >= 300 then
          BadStatus response
        else
          NetworkError
    )

  let inline fetchAs<'T> (url: string) (decoder: Decoder<'T>) (init: RequestProperties list) : JS.Promise<'T> =
    GlobalFetch.fetch(RequestInfo.Url url, requestProps init)
    |> Promise.bind (fun response ->
        if not response.Ok
        then errorString response |> failwith
        else
            response.text()
            |> Promise.map (fun res ->
                match Decode.fromString decoder res with
                | Ok successValue -> successValue
                | Error error -> failwith error))

  let inline postAs<'Data,'Result> url (decoder: Decoder<'Result>) (data:'Data) = promise {
    let options =  [
      RequestProperties.Method HttpMethod.POST
      RequestProperties.Body !^(Encode.Auto.toString(0, data))
      Fetch.requestHeaders [
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
      ]
    ]
    return! fetchAs url decoder options
  }

module Auth =
  open Fable.Import
  open Http

  [<Literal>]
  let SessionUpdatedEvent = "onSessionUpdate"
  [<Literal>]
  let SessionExpiredEvent = "onSessionExpired"

  let private notifySessionChange (user : UserData) =
    let detail =
      jsOptions<Browser.Types.CustomEventInit>(fun o ->
        o.detail <- Some (box user)
      )

    let event = Browser.Event.CustomEvent.Create(SessionUpdatedEvent, detail)
    Browser.Dom.window.dispatchEvent(event)
    |> ignore

  let private notifySessionExpired =
    let event = Browser.Event.CustomEvent.Create(SessionExpiredEvent)
    Browser.Dom.window.dispatchEvent(event)
    |> ignore


  let handleNoRetry result =
    promise {
      match result with
      | Success response ->
        let! result = response.text()
        match Decode.fromString (Thoth.Json.Decode.Auto.generateDecoder<UserData>()) result with
        | Ok successValue -> return successValue
        | Error error ->
          notifySessionExpired
          return failwith error
      | BadStatus response ->
        notifySessionExpired
        return response |> errorString |> failwith
      | NetworkError ->
        notifySessionExpired
        return failwith "Network error"
      | Unauthorized response ->
        notifySessionExpired
        let! text = unauthorizedString response
        return failwith text
    }

  // https://github.com/fable-compiler/fable-powerpack/blob/master/src/Fetch.fs
  // https://github.com/elmish/elmish/blob/master/src/cmd.fs
  let handleRefreshToken (token: RefreshTokenViewModel) (httpRequest : JWT -> JS.Promise<FetchResult>) result =
    promise {
      match result with
      | Success response -> return response
      | BadStatus response ->
        return response |> errorString |> failwith
      | NetworkError ->
        return failwith "Network error"
      | Unauthorized response ->
        let! fetchRes = fetch "/api/refreshtoken" [ RequestProperties.Method HttpMethod.POST; RequestProperties.Body !^(Encode.Auto.toString(0, token)); requestHeaders [ ContentType "application/json; charset=utf-8"; HttpRequestHeaders.Accept "application/json" ] ]
        let! res = handleNoRetry fetchRes
        let! secondRes = httpRequest res.Token
        printf "retry result %O" secondRes
        match secondRes with
        | Success response ->
            notifySessionChange res
            return response
        | BadStatus response ->
          return response |> errorString |> failwith
        | NetworkError ->
          return failwith "Network error"
        | Unauthorized response ->
          notifySessionExpired
          let! text = unauthorizedString response
          return failwith text
    }

  let private handleFetchRetry method (url : string) (decoder: Decoder<'T>) (properties: RequestProperties list) (token: RefreshTokenViewModel) : JS.Promise<'T> =
    let httpRequest (token: JWT) =
      let defaultProps =
        [ RequestProperties.Method method
          requestHeaders [ Authorization ("Bearer " + token.String)
                           ContentType "application/json; charset=utf-8"
                           Accept "application/json" ] ]
      List.append defaultProps properties
      |> fetch url

    httpRequest token.Token
    |> Promise.bind (handleRefreshToken token httpRequest)
    |> Promise.bind (fun response ->
      response.text()
      |> Promise.map (fun res ->
        match Decode.fromString decoder res with
        | Ok successValue -> successValue
        | Error error -> failwith error))

  let fetchRecord<'T> (url : string) (decoder: Decoder<'T>) (token: RefreshTokenViewModel) : JS.Promise<'T> =
    handleFetchRetry HttpMethod.GET url decoder [] token

  let postRecord<'T> (url : string) (decoder: Decoder<'T>) (token: RefreshTokenViewModel) body : JS.Promise<'T> =
    let properties = [RequestProperties.Body body]
    handleFetchRetry HttpMethod.POST url decoder properties token

  let putRecord<'T> (url : string) (decoder: Decoder<'T>) (token: RefreshTokenViewModel) body : JS.Promise<'T> =
    let properties = [RequestProperties.Body body]
    handleFetchRetry HttpMethod.PUT url decoder properties token


module BrowserHelpers =
  let inline convertToFile (input: Browser.Types.EventTarget): Browser.Types.File =
    input?files?item(0)

module Validation =
  let validateAndMapResult<'a, 'b> (errorType: (string -> 'b), value: 'a, validateFunc: ('a -> Result<'a, string>)) =
    match (validateFunc value) with
    | Success x -> None
    | Failure y -> y |> errorType |> Some

module ElmishHelpers =
  let prodLazyView comp =
    comp
    #if !DEBUG
    |> Elmish.React.Common.lazyView2
    #endif

  let tryJwtCmd (cmd: (JWT -> Cmd<'Msg> )) (userData: UserData option) =
    match userData with
    | Some x -> cmd x.Token
    | _ -> Cmd.none

  let tryRefreshJwtCmd (cmd: (RefreshTokenViewModel -> Cmd<'Msg> )) (userData: UserData option) =
    match userData with
    | Some x -> cmd { Token = x.Token; RefreshToken = x.RefreshToken }
    | _ -> Cmd.none

module ReactHelpers =
  let getValue (ev: Browser.Types.Event) =
    ev.Value

  let isLeftButtonClick (ev: Browser.Types.MouseEvent) =
    ev.button <> 0.0

module BulmaHelpers =
  open Feliz
  open Feliz.Bulma

  let inputError errorList =
    errorList
    |> List.mapi (fun index x -> Bulma.help [ prop.key (index.ToString()); color.isDanger; prop.children [ str x ] ])
