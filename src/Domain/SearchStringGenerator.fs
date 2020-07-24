namespace Domain

open System.Text.RegularExpressions;

module SearchStringGenerator =

  [<Literal>]
  let private NotValidSearchChars = @"[^-&a-z0-9\s]"

  let private replacement =  [| "a"; "a"; "a"; "a"; "a"; "a"; "c"; "e"; "e"; "e"; "e"; "i"; "i"; "i"; "i"; "n"; "o"; "o"; "o"; "o"; "o"; "ss"; "u"; "u"; "u"; "u"; "y"; "y" |]
  let private invalidChars = [| 'à'; 'á'; 'â'; 'ã'; 'ä'; 'å'; 'ç'; 'é'; 'è'; 'ê'; 'ë'; 'ì'; 'í'; 'î'; 'ï'; 'ñ'; 'ò'; 'ó'; 'ô'; 'ö'; 'õ'; 'ß';  'ù'; 'ú'; 'û'; 'ü'; 'ý'; 'ÿ' |]

  // https://stackoverflow.com/questions/34985836/index-of-element-in-array-f
  let private findIndex arr elem = arr |> Array.tryFindIndex ((=) elem)
  // https://stackoverflow.com/questions/18595597/is-using-a-stringbuilder-a-right-thing-to-do-in-f
  let private replaceInvalidChar (c: char) =
    match (findIndex invalidChars c) with
    | index when index.IsSome && index.Value > 0 -> replacement.[index.Value]
    | _ -> c.ToString()

  let private replaceInvalidChars (s: string) =
    s.ToCharArray()
    |> Array.map replaceInvalidChar
    |> String.concat ""

  let private notValidSearchWords =
    [| "as"; "at"; "the"; "was" |]
    |> Array.map (sprintf "\\b%s\\b")
    |> String.concat "|"


  let private strip regex (tagString: string) =
    Regex.Replace(tagString, regex, "")

  let GenerateSearchString (tagString: string) =
    tagString.ToLower()
    |> strip notValidSearchWords
    |> replaceInvalidChars
    |> strip NotValidSearchChars
    |> fun x -> x.Split  [|' '|]
    |> Array.filter (fun x -> (not (System.String.IsNullOrWhiteSpace x)))
    |> Array.distinct

  let FilterSearchTerms (tagString: string) =
    tagString.ToLower()
    |> replaceInvalidChars
    |> strip NotValidSearchChars
    |> System.String.Concat
