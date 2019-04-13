namespace Domain

open System.Text.RegularExpressions;

module SearchStringGenerator =

    [<Literal>]
    let private NonValidSearchChars = @"[^-&a-z0-9\s]"

    let private replacement =  [| "a"; "a"; "a"; "a"; "a"; "a"; "c"; "e"; "e"; "e"; "e"; "i"; "i"; "i"; "i"; "n"; "o"; "o"; "o"; "o"; "o"; "ss"; "u"; "u"; "u"; "u"; "y"; "y" |]
    let private invalidChars = [| 'à'; 'á'; 'â'; 'ã'; 'ä'; 'å'; 'ç'; 'é'; 'è'; 'ê'; 'ë'; 'ì'; 'í'; 'î'; 'ï'; 'ñ'; 'ò'; 'ó'; 'ô'; 'ö'; 'õ'; 'ß';  'ù'; 'ú'; 'û'; 'ü'; 'ý'; 'ÿ' |]

    // https://stackoverflow.com/questions/34985836/index-of-element-in-array-f
    let private findIndex arr elem = arr |> Array.tryFindIndex ((=) elem)
    // https://stackoverflow.com/questions/18595597/is-using-a-stringbuilder-a-right-thing-to-do-in-f
    let private ReplaceInvalidChar (c: char) =
        match (findIndex invalidChars c) with
        | index when index.IsSome && index.Value > 0 -> replacement.[index.Value]
        | _ -> c.ToString()

    let private ReplaceInvalidChars (s: string) =
        s.ToCharArray()
        |> Array.map ReplaceInvalidChar
        |> String.concat ""

    let private NotValidSearchWords =
        [| "as"; "at"; "the"; "was" |]
        |> String.concat "\\b|"
        |> sprintf "\\b|%s"

    let private Strip regex (tagString: string) =
        Regex.Replace(tagString, regex, "")

    let GenerateSearchString (tagString: string) =
        tagString.ToLower()
        |> Strip NotValidSearchWords
        |> ReplaceInvalidChars
        |> Strip NonValidSearchChars
        |> fun x -> x.Split  [|' '|]
        |> Array.filter (fun x -> ((System.String.IsNullOrWhiteSpace x) = false))
        |> Array.distinct
