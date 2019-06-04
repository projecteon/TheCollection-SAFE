module Client.Components.ComboBox.State

open System
open Elmish
open Fetch
open Thoth.Json

open Server.Api.Dtos
open Client.ElmishHelpers
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
  Cmd.OfPromise.either
    (Client.Http.fetchAs<RefValue list> (sprintf "/api/refvalues%s" (queryString model)) (Decode.Auto.generateDecoderCached<RefValue list>()))
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

let tryOnChange (model: Model) (newValue: RefValue) =
  match model.Value with
  | Some x when x.id = newValue.id ->
    let nextModel = { model with SearchTerm = None; SearchResult = None; HasFocus = false; SearchResultHoverIndex = 0 }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | _ ->
    let nextModel = { model with Value = Some newValue; SearchTerm = None; SearchResult = None; HasFocus = false; SearchResultHoverIndex = 0 }
    nextModel, Cmd.none, ExternalMsg.OnChange <| Some newValue

let tryDecreaseSearchResultHover (model: Model) =
  if model.SearchResultHoverIndex >= 0 then
    {model with SearchResultHoverIndex = model.SearchResultHoverIndex - 1 }
  else
    model

let tryIncreaseSearchResultHover (model: Model) =
  let maxIndex =
    match model.SearchResult with
    | Some y when model.Value.IsSome -> (y |> List.length) + 1
    | Some y -> y |> List.length
    | None -> 0
  if model.SearchResultHoverIndex <= maxIndex then
    {model with SearchResultHoverIndex = model.SearchResultHoverIndex + 1 }
  else
    model

let init (label: string, refValueType: RefValueTypes) =
  let initialModel = {
    Value = None
    Label = label
    SearchTerm = None
    HasFocus = false
    SearchResult = None
    SearchResultHoverIndex = 0
    DebouncedTerm = Debounce.init (TimeSpan.FromMilliseconds 300.) ""
    IsSearching = false
    RefValueType = refValueType
    Errors = []
  }
  initialModel

let update (msg : Msg) (currentModel : Model) userData : Model * Cmd<Msg> * ExternalMsg =
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
    let nextModel = { currentModel with Errors = errors }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnFocused ->
    let nextModel = { currentModel with HasFocus = true }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnBlur ->
    let nextModel = { currentModel with HasFocus = false; SearchResultHoverIndex = 0 }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | OnChange x ->
    tryOnChange currentModel x
  | SearchSuccess x ->
    let results =
      match currentModel.Value with
      | Some currentValue -> x |> List.filter (fun z -> z.id <> currentValue.id)
      | None -> x

    let nextModel = { currentModel with SearchResult = Some results; IsSearching = false }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | SearchError x ->
    let nextModel = { currentModel with IsSearching = false }
    nextModel, Cmd.none, ExternalMsg.UnChanged
  | IncreaseSearchResultHoverIndex ->
    tryIncreaseSearchResultHover currentModel, Cmd.none, ExternalMsg.UnChanged
  | DecreaseSearchResultHoverIndex ->
    tryDecreaseSearchResultHover currentModel, Cmd.none, ExternalMsg.UnChanged
  | SelectSearchResultHoverIndex ->
    let nextCmd =
      match currentModel.SearchResult with
      | Some x->
        let selectIndex = currentModel.SearchResultHoverIndex - (if currentModel.Value.IsNone then 0 else 1)
        match x |> List.tryItem selectIndex with
        | Some a -> Cmd.ofMsg (OnChange a)
        | None -> Cmd.none
      | None -> Cmd.none
    currentModel, nextCmd, ExternalMsg.UnChanged
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
    nextModel, tryJwtCmd (getCmd currentModel) userData, ExternalMsg.UnChanged
