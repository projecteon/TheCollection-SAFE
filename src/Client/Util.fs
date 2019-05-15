namespace Client

open Elmish
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Domain.SharedTypes
open Server.Api.Dtos

module Util =
  let isLeftButtonClick (ev: Fable.Import.React.MouseEvent) =
    ev.button <> 0.0

  let inline httpPost2<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
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


  let inline httpPost<'Data,'Result> (url, token: JWT option, data:'Data) = promise {
      try
          let! res = Fetch.postRecord<'Data> url data [ Fetch.requestHeaders [
            if token.IsSome then yield HttpRequestHeaders.Authorization ("Bearer " + token.Value.String)
            yield HttpRequestHeaders.ContentType "application/json; charset=utf-8"
          ] ]
          let! txt = res.text()
          return Decode.Auto.unsafeFromString<'Result> txt
      with
      | exn -> return! failwithf "Error while posting to %s. Message: %s" url exn.Message
  }

  let inline convertToFile (input: Fable.Import.Browser.EventTarget): Fable.Import.Browser.File =
    input?files?item(0)

  let tryAuthorizationRequest (cmd: (JWT -> Cmd<'Msg> )) (userData: UserData option) =
    match userData with
    | Some x -> cmd x.Token
    | _ -> Cmd.none

  let validateAndMapResult<'a, 'b> (errorType: (string -> 'b), value: 'a, validateFunc: ('a -> Result<'a, string>)) =
    match (validateFunc value) with
    | Success x -> None
    | Failure y -> y |> errorType |> Some

module ReactHelpers =
  open Fable.Helpers.React
  open Fable.Helpers.React.Props
  let getValue (ev: Fable.Import.React.FormEvent) = ev.Value

module FulmaHelpers =
  open Fable.Helpers.React
  open Fable.Helpers.React.Props
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
