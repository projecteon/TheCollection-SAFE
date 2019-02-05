module Client.Components.ComboBox.Types

open System.Text.RegularExpressions
open Elmish

open Services.Dtos

let ReplaceWhiteSpaceRegex = new Regex(@"\s+")
let ReplaceWhitespace value =
  ReplaceWhiteSpaceRegex.Replace(value, "")
let lowercase (x : string) = x.ToLower()

type Model = {
  Value: RefValue option
  Label: string
  SearchTerm: string option
  HasFocus: bool
  SearchResult: Option<RefValue list>
  DebouncedTerm : Elmish.Debounce.Model<string>
  IsSearching : bool
  ApiPath: string
}

type Msg =
| Clear
| Init of RefValue option
| OnSearchTermChange of string
| OnFocused
| OnBlur
| OnChange of RefValue
| SearchSuccess of RefValue list
| SearchError of exn
| DebounceSearchTermMsg of Debounce.Msg<string>
| FetchSuggestions of string
