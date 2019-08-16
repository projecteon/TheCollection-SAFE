module Client.Teabags.Types

open Domain.SharedTypes
open Server.Api.Dtos

type SearchResult = {
  data: Teabag list
  searchCount: int64
}

type SearchSessionData = {searchedTerms: string option; result: SearchResult<Teabag list>}

type Model = {
  result: Teabag list
  resultCount: int option
  searchedTerms: string option
  searchError: string option
  zoomImageId: ImageId option
  page: int64
  isLoading: bool
}

type Msg =
| OnSearchTermChange of string
| Search
| SearchError of exn
| SearchSuccess of SearchResult<Teabag list>
| SearchTermChanged
| SearchTermError
| ZoomImageToggle of ImageId option

type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

let validateSearchTerm model =
  match model.searchedTerms with
  | Some x ->
    match x.Length with
    | i when i <= 2  -> Failure "Searchstring must be 3 characters or more."
    | _ -> Success x
  | _ -> Failure "Searchstring must be 3 characters or more."