module Client.Teabags.State

open Elmish
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Client.Util
open Client.Teabags.Types
open Services.Dtos

let queryString model =
    match model.searchedTerms with
    | Some x -> sprintf "?term=%s&page=%i" x model.page
    | _ -> ""

let getTeabagsCmd model (token: JWT) =
    Cmd.ofPromise
        (Fetch.fetchAs<SearchResult<Teabag list>> (sprintf "/api/teabags%s" (queryString model)) (Decode.Auto.generateDecoder<SearchResult<Teabag list>>()) )
        [Fetch.requestHeaders [
          HttpRequestHeaders.Authorization ("Bearer " + token.String)
          HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        ]]
        SearchSuccess
        SearchError

let init (userData: UserData option) =
    let initialModel = {
      result = []
      resultCount=None
      searchedTerms=None
      searchError=None
      zoomImageId = None
      page = int64 0
      isLoading = false
      userData = userData
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
      | Success x -> {model with searchError = None; result = []; resultCount=None; isLoading=true}, tryAuthorizationRequest (getTeabagsCmd model) model.userData
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

