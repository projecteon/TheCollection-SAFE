namespace TeaCollection.Infrastructure.MsSql

open System
open Search

module Util =
    let extractSearchTerm (term: SearchTerm) =
        match term with
        | Some x -> x
        | _ -> ""

    let lowercase (x : string) = x.ToUpper()
    let split (value: string) =
      value.Split [|' '|]
    
    let createJsonArray (terms: string array) =
        match terms with
        | [||] -> "[]"
        | y -> y
            |> String.concat "\",\""
            |> sprintf "[\"%s\"]"
    
    let extractTerms (term: SearchTerm) =
        match term with
        | None -> [||]
        | Some x when x = "" -> [||]
        | Some x -> x |> split

    let createJsonTermArray (term: SearchTerm) =
        term
        |> extractTerms
        |> createJsonArray
                

    let mapOptionalStringValue (optionalValue) =
      match optionalValue with
      | Some x -> x
      | None -> ""

    let mapOptionalIntValue (optionalValue) =
      match optionalValue with
      | Some x -> x
      | None -> 0

    let mapOptionalDateTimeValue (optionalValue) =
      match optionalValue with
      | Some x -> x
      | None -> DateTime.MinValue

