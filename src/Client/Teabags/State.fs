module Client.Teabags.State

open Elmish
open Thoth.Json

open Client.ElmishHelpers
open Client.Teabags.Types
open Server.Api.Dtos

let queryString model =
    match model.searchedTerms with
    | Some x -> sprintf "?term=%s&page=%i" x model.page
    | _ -> ""

let getTeabagsCmd model (token: RefreshTokenViewModel) =
    Cmd.OfPromise.either
        (Client.Auth.fetchRecord<SearchResult<Teabag list>> (sprintf "/api/teabags%s" (queryString model)) (Decode.Auto.generateDecoder<SearchResult<Teabag list>>()))
        token
        SearchSuccess
        SearchError

let init =
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

let update (msg:Msg) model userData : Model*Cmd<Msg> =
    match msg with
    | OnSearchTermChange searchTerms ->
      {model with searchedTerms = Some searchTerms; searchError = None}, Cmd.none
    | Search ->
      let result = validateSearchTerm model
      match result with
      | Success x -> {model with searchError = None; result = []; resultCount=None; isLoading=true}, tryRefreshJwtCmd (getTeabagsCmd model) userData
      | Failure x -> {model with searchError = Some x}, Cmd.none
    | SearchSuccess result ->
        { model with result = result.data; resultCount=Some result.count; isLoading=false }, Cmd.none
    | SearchError exn ->
        printfn "search error %O" exn
        { model with isLoading=false }, Cmd.none
    | ZoomImageToggle id ->
        { model with zoomImageId = id }, Cmd.none
    | _ -> model, Cmd.none
