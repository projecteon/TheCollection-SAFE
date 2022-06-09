module Client.Teabags.State

open Elmish
open Thoth.Json

open Client.ElmishHelpers
open Client.Teabags.Types
open Server.Api.Dtos

[<Literal>]
let private SearchDataKey = "thecollection:search"

let private loadSearch () : SearchSessionData =
  let searchDecoder = Thoth.Json.Decode.Auto.generateDecoder<SearchSessionData>()
  match Client.SessionStorage.load searchDecoder SearchDataKey with
  | Ok searchData -> searchData
  | Error _ -> {searchedTerms = None; result = {count = 0; data = []}}

let private saveSearchData (searchData: SearchSessionData) =
  (Client.SessionStorage.save SearchDataKey) searchData

let clearSearchedData () =
  Client.SessionStorage.delete SearchDataKey

let queryString model =
  match model.searchedTerms with
  | Some x -> sprintf "?term=%s&page=%i" x model.page
  | _ -> ""

let getTeabagsCmd model (token: RefreshTokenViewModel) =
  Cmd.OfPromise.either
    (Client.Auth.fetchRecord<SearchResult<Teabag list>> (sprintf "/api/teabags%s" (queryString { model with page = model.page - 1 })) (Decode.Auto.generateDecoder<SearchResult<Teabag list>>()))
    token
    SearchSuccess
    SearchError

let init() =
  let sessionData = loadSearch();
  let initialModel = {
    result = sessionData.result.data
    resultCount= if sessionData.result.count = 0 then None else Some sessionData.result.count
    searchedTerms=sessionData.searchedTerms
    searchError=None
    zoomImageId = None
    page = 1
    isLoading = false
  }
  initialModel

let update (msg:Msg) model userData : Model*Cmd<Msg> =
  match msg with
  | OnSearchTermChange searchTerms ->
    let filteredSearchTerms = Domain.SearchStringGenerator.FilterSearchTerms searchTerms
    {model with searchedTerms = Some filteredSearchTerms; searchError = None}, Cmd.none
  | PerformSearch ->
    {model with result = []; isLoading=true}, tryRefreshJwtCmd (getTeabagsCmd model) userData
  | Search ->
    let result = validateSearchTerm model
    match result with
    | Success x -> {model with searchError = None; resultCount=None; page=1}, Cmd.ofMsg PerformSearch
    | Failure x -> {model with searchError = Some x}, Cmd.none
  | SearchSuccess result ->
    saveSearchData {searchedTerms = model.searchedTerms; result = result}
    { model with result = result.data; resultCount=Some result.count; isLoading=false }, Cmd.none
  | SearchError exn ->
    printfn "search error %O" exn
    { model with isLoading=false }, Cmd.none
  | ZoomImageToggle id ->
    { model with zoomImageId = id }, Cmd.none
  | SearchPageCmd msg ->
    let nextModel =
      match msg with
      | Client.Components.Pagination.ExternalMsg.SearchPageNext -> { model with page = model.page + 1 }
      | Client.Components.Pagination.ExternalMsg.SearchPagePrevious -> { model with page = model.page - 1 }
      | Client.Components.Pagination.ExternalMsg.SearchPage page -> { model with page = page }

    nextModel, Cmd.ofMsg PerformSearch
  | _ -> model, Cmd.none
