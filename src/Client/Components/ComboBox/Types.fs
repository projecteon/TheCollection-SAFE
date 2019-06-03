module Client.Components.ComboBox.Types

open System.Text.RegularExpressions
open Elmish

open Server.Api.Dtos
open Domain.SharedTypes

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
  SearchResultHoverIndex: int
  DebouncedTerm : Elmish.Debounce.Model<string>
  IsSearching : bool
  RefValueType: RefValueTypes
  Errors: string seq
}

type ExternalMsg =
  | UnChanged
  | OnChange of RefValue option

type Msg =
| Clear
| SetValue of RefValue option
| SetErrors of string seq
| OnSearchTermChange of string
| OnFocused
| OnBlur
| OnChange of RefValue
| IncreaseSearchResultHoverIndex
| DecreaseSearchResultHoverIndex
| SelectSearchResultHoverIndex
| SearchSuccess of RefValue list
| SearchError of exn
| DebounceSearchTermMsg of Debounce.Msg<string>
| FetchSuggestions of string
