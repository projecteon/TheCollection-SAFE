module Client.Components.ComboBox.State

open System
open Elmish
open Fable.PowerPack
open Thoth.Json

open Services.Dtos
open Client.Components.ComboBox.Types

let queryString model =
  match model.SearchTerm with
  | Some x -> sprintf "?term=%s" x
  | _ -> ""

// https://github.com/SAFE-Stack/SAFE-Search
// https://github.com/iyegoroff/fable-import-debounce
let getCmd model =
  Cmd.ofPromise
    (Fetch.fetchAs<RefValue list> (sprintf "%s%s" model.ApiPath (queryString model)) (Decode.Auto.generateDecoder<RefValue list>()) )
    []
    SearchSuccess
    SearchError


let setValueCmd (refValue: RefValue option) =
  Cmd.ofMsg (Init refValue)

let init (label: string, apiPath: string) =
  let initialModel = {
    Value = None
    Label = label
    SearchTerm = None
    HasFocus = false
    SearchResult = None
    DebouncedTerm = Debounce.init (TimeSpan.FromMilliseconds 300.) ""
    IsSearching = false
    ApiPath = apiPath
  }
  initialModel

// Fable.Import.Browser.window.alert()

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
  match msg with
  | Clear ->
    let nextModel = { currentModel with Value = None; SearchTerm = None; SearchResult = None; HasFocus = false }
    nextModel, Cmd.none
  | OnSearchTermChange x ->
    if x.Equals(currentModel.SearchTerm.Value)
      then currentModel, Cmd.none
    else
      let nextModel = { currentModel with SearchTerm = Some x }
      nextModel, Debounce.inputCmd x DebounceSearchTermMsg
  | Init x ->
    let nextModel = { currentModel with Value = x }
    nextModel, Cmd.none
  | OnFocused ->
    let nextModel = { currentModel with HasFocus = true }
    nextModel, Cmd.none
  | OnBlur ->
    let nextModel = { currentModel with HasFocus = false }
    nextModel, Cmd.none
  | OnChange x ->
    let nextModel = { currentModel with Value = Some x; SearchTerm = None; SearchResult = None; HasFocus = false }
    nextModel, Cmd.none
  | SearchSuccess x ->
    let nextModel = { currentModel with SearchResult = Some x; IsSearching = false }
    nextModel, Cmd.none
  | SearchError x ->
    let nextModel = { currentModel with IsSearching = false }
    nextModel, Cmd.none
  | DebounceSearchTermMsg msg ->
    Debounce.updateWithCmd
      msg
      DebounceSearchTermMsg
      currentModel.DebouncedTerm
      (fun x -> { currentModel with DebouncedTerm = x })
      (FetchSuggestions >> Cmd.ofMsg)
  | FetchSuggestions text ->
    let nextModel = { currentModel with IsSearching = true }
    nextModel, getCmd currentModel
