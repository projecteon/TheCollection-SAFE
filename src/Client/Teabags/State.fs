module Client.Teabags.State

open Elmish
open Fable.PowerPack
open Thoth.Json

open Client.Teabags.Types
open Services.Dtos
open Domain.Types

let queryString model =
    match model.searchedTerms with
    | Some x -> sprintf "?term=%s&page=%i" x model.page
    | _ -> ""

let getTeabagsCmd model =
    Cmd.ofPromise
        (Fetch.fetchAs<SearchResult<Teabag list>> (sprintf "/api/teabags%s" (queryString model)) (Decode.Auto.generateDecoder<SearchResult<Teabag list>>()) )
        []
        SearchSuccess
        SearchError

let init () =
    let initialModel = {
      result = []
      resultCount=None
      searchedTerms=None
      searchError=None
      zoomImageId = None
      page = int64 0
      isLoading = false
    }
    initialModel

let update (msg:Msg) model : Model*Cmd<Msg> =
    printfn "teabags update"
    match msg with
    | OnSearchTermChange searchTerms ->
      {model with searchedTerms = Some searchTerms; searchError = None}, Cmd.none
    | Search ->
      let result = validateSearchTerm model
      match result with
      | Success x -> {model with searchError = None; result = []; resultCount=None; isLoading=true}, getTeabagsCmd model
      | Failure x -> {model with searchError = Some x}, Cmd.none
    | SearchSuccess result ->
        printfn "teabags update"
        { model with result = result.data; resultCount=Some result.count; isLoading=false }, Cmd.none
    | SearchError exn ->
        printfn "search error %O" exn
        { model with isLoading=false }, Cmd.none
    | ZoomImageToggle id ->
        { model with zoomImageId = id }, Cmd.none
    | _ -> model, Cmd.none

