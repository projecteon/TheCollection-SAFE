namespace Client

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

  let inline post2<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
      let body = Encode.Auto.toString(0, data)
      let props = [
          RequestProperties.Method HttpMethod.POST
          Fetch.requestHeaders [
              if token.IsSome then yield HttpRequestHeaders.Authorization ("Bearer " + token.Value.String)
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

  let fetch2 (url: string) (init: RequestProperties list) : JS.Promise<Response> =
    GlobalFetch.fetch(RequestInfo.Url url, requestProps init)
    |> Promise.map (fun response ->
        if response.Ok
        then response
        else errorString response |> failwith)

  let inline private sendRecord (url: string) (record:'T) (properties: RequestProperties list) httpMethod : JS.Promise<Response> =
    let defaultProps =
        [ RequestProperties.Method httpMethod
          requestHeaders [ContentType "application/json"]
          RequestProperties.Body !^(Encode.Auto.toString(0, record))]
    // Append properties after defaultProps to make sure user-defined values
    // override the default ones if necessary
    List.append defaultProps properties
    |> fetch2 url

  let inline postRecord<'T> (url: string) (record:'T) (properties: RequestProperties list) : JS.Promise<Response> =
    sendRecord url record properties HttpMethod.POST

  let inline post<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
      try
          let! res = postRecord<'Data> url data [ Fetch.requestHeaders [
            if token.IsSome then yield HttpRequestHeaders.Authorization ("Bearer " + token.Value.String)
            yield HttpRequestHeaders.ContentType "application/json; charset=utf-8"
          ] ]
          let! txt = res.text()
          return Decode.Auto.unsafeFromString<'Result> txt
      with
      | exn -> return! failwithf "Error while posting to %s. Message: %s" url exn.Message
  }

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
        let! res = post<RefreshTokenViewModel, UserData>("/api/refreshtoken", None, token)
        let! secondRes = httpRequest res.Token
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

  let fetchRecord<'T> (url : string) (token: RefreshTokenViewModel) (decoder: Decoder<'T>) : JS.Promise<'T> =
    let httpRequest (token: JWT) =
      let defaultProps =
        [ RequestProperties.Method HttpMethod.GET
          requestHeaders [ ContentType "application/json; charset=utf-8"
                           Authorization ("Bearer " + token.String) ] ]
      defaultProps
      |> fetch url

    httpRequest token.Token
    |> Promise.bind (handleRefreshToken token httpRequest)
    |> Promise.bind (fun response ->
      response.text() 
      |> Promise.map (fun res ->
        match Decode.fromString decoder res with
        | Ok successValue -> successValue
        | Error error -> failwith error))
              

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

  let tryRefresjJwtCmd (cmd: (RefreshTokenViewModel -> Cmd<'Msg> )) (userData: UserData option) =
    match userData with
    | Some x -> cmd { Token = x.Token; RefreshToken = x.RefreshToken }
    | _ -> Cmd.none

module ReactHelpers =
  let getValue (ev: Browser.Types.Event) =
    ev.Value

  let isLeftButtonClick (ev: Browser.Types.MouseEvent) =
    ev.button <> 0.0

module FulmaHelpers =
  open Fable.React.Props
  open Fulma

  let inputError errorList =
    errorList
    |> List.mapi (fun index x -> Help.help [ Help.Props [Key (index.ToString())]; Help.Color IsDanger ] [ str x ])

module ReChartHelpers =
  type DataCount = Ten=10 | Twenty=20

  let LineColors = [|"#0088FE"; "#00C49F"; "#FFBB28"; "#FF8042"; "#8884d8"|];
  let C3ColorsExtended = [|"#1f77b4"; "#aec7e8"; "#ff7f0e"; "#ffbb78"; "#2ca02c"; "#98df8a"; "#d62728"; "#ff9896"; "#9467bd"; "#c5b0d5"; "#8c564b"; "#c49c94"; "#e377c2"; "#f7b6d2"; "#7f7f7f"; "#c7c7c7"; "#bcbd22"; "#dbdb8d"; "#17becf"; "#9edae5" |];
  let C3Colors = [|"#1f77b4"; "#ff7f0e"; "#2ca02c"; "#d62728"; "#9467bd"; "#8c564b"; "#e377c2"; "#7f7f7f"; "#bcbd22"; "#17becf" |];

  let getColor (colors: string array) x =
    let colorCount = C3Colors |> Array.length
    let rec tailRecursiveFactorial acc =
      if acc < colorCount then
        colors.[acc]
      else
        tailRecursiveFactorial (acc - colorCount)
    tailRecursiveFactorial x

  
  let margin t r b l =
      Fable.Recharts.Props.Chart.Margin { top = t; bottom = b; right = r; left = l }
