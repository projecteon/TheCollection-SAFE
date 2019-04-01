module Client.Components.ComboBox.State

open System
open Elmish
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Services.Dtos
open Client.Util
open Client.Components.ComboBox.Types
open Domain.SharedTypes

let queryString model =
  match model.SearchTerm with
  | Some x -> sprintf "?term=%s&refvaluetype=%O" x model.RefValueType
  | _ -> sprintf "?refvaluetype=%O" model.RefValueType

// https://github.com/SAFE-Stack/SAFE-Search
// https://github.com/iyegoroff/fable-import-debounce
// https://mangelmaxime.github.io/Thoth/json/v2/decode.html#auto-decoder
let getCmd model (token: JWT) =
  Cmd.ofPromise
    (Fetch.fetchAs<RefValue list> (sprintf "/api/refvalues%s" (queryString model)) (Decode.Auto.generateDecoder<RefValue list>()) )
    [Fetch.requestHeaders [
      HttpRequestHeaders.Authorization ("Bearer " + token.String)
      HttpRequestHeaders.ContentType "application/json; charset=utf-8"
    ]]
    SearchSuccess
    SearchError
    
let setValueCmd (refValue: RefValue option) =
  Cmd.ofMsg (SetValue refValue)

let setErrors (errors: string seq) =
  Cmd.ofMsg (SetErrors errors)

let init (label: string, refValueType: RefValueTypes, userData: UserData option) =
  let initialModel = {
    Value = None
    Label = label
    SearchTerm = None
    HasFocus = false
    SearchResult = None
    DebouncedTerm = Debounce.init (TimeSpan.FromMilliseconds 300.) ""
    IsSearching = false
    RefValueType = refValueType
    userData = userData
    Errors = []
  }
  initialModel

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> * ExternalMsg =
  match msg with
  | Clear ->
    let nextModel = { currentModel with Value = None; SearchTerm = None; SearchResult = None; HasFocus = false }
    nextModel, Cmd.none, ExternalMsg.OnChange None
  | OnSearchTermChange x ->
    if x.Equals(currentModel.SearchTerm.Value)
      then currentModel, Cmd.none, ExternalMsg.UnChanged
    else
      let nextModel = { currentModel with SearchTerm = Some x }
      nextModel, Debounce.inputCmd x DebounceSearchTermMsg, ExternalMsg.UnChanged
  | SetValue x ->
    let nextModel = { currentModel with Value = x }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | SetErrors errors ->
    printf "update %O" (errors |> List.ofSeq)
    let nextModel = { currentModel with Errors = errors }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnFocused ->
    let nextModel = { currentModel with HasFocus = true }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnBlur ->
    let nextModel = { currentModel with HasFocus = false }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnChange x ->
    let nextModel = { currentModel with Value = Some x; SearchTerm = None; SearchResult = None; HasFocus = false }
    nextModel, Cmd.none, ExternalMsg.OnChange <| Some x
  | SearchSuccess x ->
    let nextModel = { currentModel with SearchResult = Some x; IsSearching = false }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | SearchError x ->
    let nextModel = { currentModel with IsSearching = false }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | DebounceSearchTermMsg msg ->
    let (dModel, dMsg) = Debounce.updateWithCmd
                          msg
                          DebounceSearchTermMsg
                          currentModel.DebouncedTerm
                          (fun x -> { currentModel with DebouncedTerm = x })
                          (FetchSuggestions >> Cmd.ofMsg)
    dModel, dMsg, ExternalMsg.UnChanged
  | FetchSuggestions text ->
    let nextModel = { currentModel with IsSearching = true }
    nextModel, tryAuthorizationRequest (getCmd currentModel) currentModel.userData, ExternalMsg.UnChanged
