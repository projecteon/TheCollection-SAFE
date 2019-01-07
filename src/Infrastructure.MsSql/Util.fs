namespace TeaCollection.Infrastructure.MsSql

open Search

module Util =
    let extractSearchTerm (term: SearchTerm) =
        match term with
        | Some x -> x
        | _ -> ""

    let lowercase (x : string) = x.ToUpper()
    let split (value: string) =
      value.Split [|' '|] |> Array.toSeq

    let extractSearchTerm3 (term: SearchTerm) =
      match term with
      | None -> "[]"
      | Some x ->
        x |> lowercase |> split
        |> Seq.length
        |> function
          | 0 -> "[]"
          | _ -> x |> split |> String.concat "\",\"" |> sprintf "[\"%s\"]"

    let extractSearchTerm2 (term: SearchTerms option) =
        match term with
        | Some x -> x
        | _ -> []
        |> function
            | [] -> "[]"
            | y -> y
                    |> String.concat "\",\""
                    |> sprintf "[\"%s\"]"

    let mapOptionalStringValue (optionalValue) =
      match optionalValue with
      | Some x -> x
      | None -> ""
    let mapOptionalIntValue (optionalValue) =
      match optionalValue with
      | Some x -> x
      | None -> 0
    // let appendPrefixLike (term: SearchTerm) =
    //     match term.[0] with
    //     | '%' -> term
    //     | _ -> sprintf "%%%s" term

    // let appendSuffixLike (term: SearchTerm) =
    //     match term.[term.Length - 1] with // https://stackoverflow.com/questions/4157875/better-way-to-get-the-last-character-in-a-string-in-f
    //     | '%' -> term
    //     | _ -> sprintf "%s%%" term

    // let appendLike (term: SearchTerm) =
    //     term |> appendPrefixLike |> appendSuffixLike

