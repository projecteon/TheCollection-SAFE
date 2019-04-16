module Client.Components.ComboBox.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fulma

open Services.Dtos
open Domain.SharedTypes
open Client.Components.ComboBox.Types
open Elmish.React

let CarrigeReturnKeyCode = 13.0
let ArrowUpKeyCode = 38.0
let ArrowDownKeyCode = 40.0

let onChange dispatch (ev: Fable.Import.React.MouseEvent) (newValue: RefValue) =
  let isLeftButtonClick = ev.button = 0.0;
  if isLeftButtonClick then
    dispatch (OnChange newValue)
  else
    ev.stopPropagation()
    ev.preventDefault() |> ignore

let onKeyDown (model: Model) dispatch (ev: Fable.Import.React.KeyboardEvent) =
  if model.HasFocus = false && model.SearchResult.IsNone && model.Value.IsNone then
    ev |> ignore
  else
    let isUpOrDownArrow = [|ArrowDownKeyCode; ArrowUpKeyCode|] |> Array.tryFindIndex ((=) ev.keyCode) |> function | None -> false | _ -> true
    if isUpOrDownArrow then
      ev.preventDefault();
      ev.stopPropagation();

    if ev.keyCode = ArrowUpKeyCode then
      dispatch DecreaseSearchResultHoverIndex |> ignore
    else if ev.keyCode = ArrowDownKeyCode then
      dispatch IncreaseSearchResultHoverIndex |> ignore
    else if ev.keyCode = CarrigeReturnKeyCode then
      dispatch SelectSearchResultHoverIndex |> ignore
      (ev.currentTarget :?> Fable.Import.Browser.HTMLInputElement).blur()
    else
      ev |> ignore

let getDisplayValue (model: Model) =
  match model.HasFocus with
  | true ->
    match model.SearchTerm with
    | Some x -> x
    | _ -> ""
  | false ->
    match model.Value with
    | Some x -> x.description
    | _ -> ""

let private hasValue (value: RefValue option) =
  match value with
  | Some x ->
    match x.id with
    | DbId y when y = 0 -> false
    | _ -> true
  | None -> false

let private getHoveredStyle index  searchResultHoverIndex =
  if index = searchResultHoverIndex then
    [ CSSProp.BackgroundColor "whitesmoke"; CSSProp.Color "#0a0a0a" ]
  else
    []

let viewChoices (model: Model) (dispatch : Msg -> unit) =
  match model.HasFocus with
  | true when model.Value.IsSome || model.SearchResult.IsSome ->
    Dropdown.dropdown [ Dropdown.CustomClass "isDisplayed" ] [
      div [] []
      Dropdown.menu [ ] [
        Dropdown.content [ ] [
          match model.Value with
          | Some x when x.id.Int > 0 ->
            yield Dropdown.Item.a [ Dropdown.Item.Props [ Key (x.id.ToString()); OnMouseDown (fun ev -> onChange dispatch ev x); Style (getHoveredStyle 0 model.SearchResultHoverIndex) ] ] [ str x.description ]
            if model.SearchResult.IsSome then
              yield Dropdown.divider [ ]
          | _ -> ()

          match model.SearchResult with
          | Some results ->
            yield results
            |> List.mapi (fun index refvalue -> Dropdown.Item.a [ Dropdown.Item.Props [ Key (refvalue.id.ToString()); OnMouseDown (fun ev -> onChange dispatch ev refvalue); Style (getHoveredStyle (if model.Value.IsSome then index + 1 else index) model.SearchResultHoverIndex) ] ] [ str refvalue.description ])
            |> ofList
          | _ -> ()
        ]
      ]
    ]
  | _ -> span [] []

let inputIcon model =
  match model.Value with
  | Some x -> Icon.icon [
        yield Icon.Size IsSmall
        yield Icon.IsRight
        if model.Value |> hasValue then
          yield Icon.Modifiers [Modifier.TextColor IsSuccess]] [
        Fa.i [ match model.HasFocus, model.IsSearching with
                | true, false -> yield Fa.Solid.Search
                | true, true -> yield Fa.Solid.CircleNotch
                | _ -> yield Fa.Solid.Check ] [ ]

    ]
  | None -> Fable.Helpers.React.nothing

let inputElement model dispatch =
  Input.text [
    yield Input.Option.Id (model.Label |> lowercase |> ReplaceWhitespace)
    yield Input.Placeholder (sprintf "Ex: Some %s" (model.Label |> lowercase |> ReplaceWhitespace))
    yield Input.ValueOrDefault (getDisplayValue model)
    yield Input.OnChange (fun ev -> dispatch (OnSearchTermChange ev.Value))
    yield Input.Props [
      DOMAttr.OnFocus (fun ev -> dispatch OnFocused)
      DOMAttr.OnBlur (fun ev -> dispatch OnBlur)
      DOMAttr.OnKeyDown (fun ev -> onKeyDown model dispatch ev)
    ]
    if (not (Seq.isEmpty model.Errors)) then
      yield Input.Color IsDanger
    else if model.Value |> hasValue then
      yield Input.Color IsSuccess
  ]

let viewWithButtons (model : Model) (dispatch : Msg -> unit) =
  Field.div [ ] [
    Label.label [ Label.Option.For (model.Label |> lowercase |> ReplaceWhitespace) ] [
      str model.Label
    ]
    Field.div [ Field.HasAddons ] [
      Control.p [ ] [
        Button.button [ Button.Disabled (model.Value.IsNone); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Clear)  ] [ Icon.icon [ ] [ Fa.i [ Fa.Solid.Times ][] ] ]
      ]
      Control.div [ Control.IsExpanded; Control.HasIconRight ] [
        inputElement model dispatch
        inputIcon model
        (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
        viewChoices model dispatch
      ]
    ]
  ]

let lazyViewWithButtons = Common.lazyView2 viewWithButtons

let viewWithoutButtons (model : Model) (dispatch : Msg -> unit) =
  Field.div [ ] [
    Label.label [ Label.Option.For (model.Label |> lowercase |> ReplaceWhitespace) ] [
      str model.Label
    ]
    Control.div [ Control.IsExpanded; Control.HasIconRight ] [
      inputElement model dispatch
      inputIcon model
      (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
      viewChoices model dispatch
    ]
  ]

let lazyViewWithoutButtons = Common.lazyView2 lazyViewWithButtons

let viewWithCustomGrouped customComp (model : Model) (dispatch : Msg -> unit) =
  Field.div [ ] [
    Label.label [ Label.Option.For (model.Label |> lowercase |> ReplaceWhitespace) ] [
      str model.Label
    ]
    Field.div [ Field.IsGrouped ] [
      Control.div [ Control.IsExpanded; Control.HasIconRight ] [
        inputElement model dispatch
        inputIcon model
        (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
        viewChoices model dispatch
      ]
      customComp
    ]
  ]

let lazyViewWithCustomGrouped customComp = Common.lazyView2 (viewWithCustomGrouped customComp)