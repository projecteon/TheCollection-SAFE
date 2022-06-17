module Client.Components.ComboBox.View

open Elmish.React
open Fable.React
open Fable.FontAwesome
open Feliz
open Feliz.Bulma

open Domain.SharedTypes
open Client.Components.ComboBox.Types

let CarrigeReturnKeyCode = "Enter"
let ArrowUpKeyCode = "ArrowUp"
let ArrowDownKeyCode = "ArrowDown"

let onChange dispatch (ev: Browser.Types.MouseEvent) (newValue: RefValue) =
  let isLeftButtonClick = ev.button = 0.0;
  if isLeftButtonClick then
    dispatch (OnChange newValue)
  else
    ev.stopPropagation()
    ev.preventDefault() |> ignore

let onKeyDown (model: Model) dispatch (ev: Browser.Types.KeyboardEvent) =
  if (not model.HasFocus) && model.SearchResult.IsNone && model.Value.IsNone then
    ev |> ignore
  else
    let isUpOrDownArrow = [|ArrowDownKeyCode; ArrowUpKeyCode|] |> Array.tryFindIndex ((=) ev.code) |> function | None -> false | _ -> true
    if isUpOrDownArrow then
      ev.preventDefault();
      ev.stopPropagation();

    if ev.code = ArrowUpKeyCode then
      dispatch DecreaseSearchResultHoverIndex |> ignore
    else if ev.code = ArrowDownKeyCode then
      dispatch IncreaseSearchResultHoverIndex |> ignore
    else if ev.code = CarrigeReturnKeyCode then
      dispatch SelectSearchResultHoverIndex |> ignore
      (ev.currentTarget :?> Browser.Types.HTMLInputElement).blur()
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
    [ style.backgroundColor "whitesmoke"; style.color "#0a0a0a" ]
  else
    []

let viewChoices (model: Model) (dispatch : Msg -> unit) =
  match model.HasFocus with
  | true when model.Value.IsSome || model.SearchResult.IsSome ->
    Bulma.dropdown [
      prop.className "isDisplayed"
      prop.children [
        Html.div []
        Bulma.dropdownMenu [
          Bulma.dropdownContent [
            match model.Value with
            | Some x when x.id.Int > 0 ->
              Bulma.dropdownItem.a [ prop.key (x.id.ToString()); prop.onMouseDown (fun ev -> onChange dispatch ev x); prop.style (getHoveredStyle 0 model.SearchResultHoverIndex); prop.text x.description ] 
              if model.SearchResult.IsSome then
                Bulma.dropdownDivider [ ]
            | _ -> ()

            match model.SearchResult with
            | Some results ->
              results
              |> List.mapi (fun index refvalue -> Bulma.dropdownItem.a [ prop.key (refvalue.id.ToString()); prop.onMouseDown (fun ev -> onChange dispatch ev refvalue); prop.style (getHoveredStyle (if model.Value.IsSome then index + 1 else index) model.SearchResultHoverIndex); prop.text refvalue.description ])
              |> ofList
            | _ -> ()
          ]
        ]
      ]
    ]
  | _ -> nothing

let inputIcon model =
  match model.Value with
  | Some x -> Bulma.icon [
                Bulma.icon.isSmall
                Bulma.icon.isRight
                if model.Value |> hasValue then
                  Bulma.color.hasTextSuccess
                prop.children [
                  Fa.i [ match model.HasFocus, model.IsSearching with
                          | true, false -> Fa.Solid.Search
                          | true, true -> Fa.Solid.CircleNotch
                          | _ -> Fa.Solid.Check ] [ ]
                ]
              ]
  | None -> nothing

let inputElement model dispatch =
  Bulma.input.text [
    prop.id (model.Label |> lowercase |> ReplaceWhitespace)
    prop.placeholder (sprintf "Ex: Some %s" (model.Label |> lowercase |> ReplaceWhitespace))
    prop.valueOrDefault (getDisplayValue model)
    prop.onChange (fun value -> dispatch (OnSearchTermChange value))
    prop.onFocus (fun _ -> dispatch OnFocused)
    prop.onBlur (fun _ -> dispatch OnBlur)
    prop.onKeyDown (fun ev -> onKeyDown model dispatch ev)
    if (not (Seq.isEmpty model.Errors)) then  
      color.isDanger
    else if model.Value |> hasValue then
      color.isSuccess
  ]

let viewWithButtons (model : Model) (dispatch : Msg -> unit) =
  Bulma.field.div [
    Bulma.label [ prop.for' (model.Label |> lowercase |> ReplaceWhitespace); prop.text model.Label ]
    Bulma.field.div [
      Bulma.field.hasAddons
      prop.children [
        Bulma.control.p [
          Bulma.button.button [ prop.disabled (model.Value.IsNone); prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Clear); prop.children [ Bulma.icon [ Fa.i [ Fa.Solid.Times ] [] ] ] ]
        ]
        Bulma.control.div [
          Bulma.control.isExpanded
          Bulma.control.hasIconsRight
          prop.children [
            inputElement model dispatch
            inputIcon model
            (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
            viewChoices model dispatch
          ]
        ]
      ]
    ]
  ]

let lazyViewWithButtons = Common.lazyView2 viewWithButtons

let viewWithoutButtons (model : Model) (dispatch : Msg -> unit) =
  Bulma.field.div [
    Bulma.label [ prop.for' (model.Label |> lowercase |> ReplaceWhitespace); prop.text model.Label ]
    Bulma.control.div [
      Bulma.control.isExpanded
      Bulma.control.hasIconsRight
      prop.children [
        inputElement model dispatch
        inputIcon model
        (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
        viewChoices model dispatch
      ]
    ]
  ]

let lazyViewWithoutButtons = Common.lazyView2 lazyViewWithButtons

let viewWithCustomGrouped customComp (model : Model) (dispatch : Msg -> unit) =
  Bulma.field.div [
    Bulma.label [ prop.for' (model.Label |> lowercase |> ReplaceWhitespace); prop.text model.Label ]
    Bulma.field.div [
      Bulma.field.hasAddons
      prop.children [
        Bulma.control.p [
          Bulma.button.button [ prop.disabled (model.Value.IsNone); prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Clear); prop.children [ Bulma.icon [ Fa.i [ Fa.Solid.Times ] [] ] ] ]
        ]
        Bulma.control.div [
          Bulma.control.isExpanded
          Bulma.control.hasIconsRight
          prop.children [
            inputElement model dispatch
            inputIcon model
            (Client.FulmaHelpers.inputError (model.Errors |> List.ofSeq)) |> ofList
            viewChoices model dispatch
          ]
        ]
        customComp
      ]
    ]
  ]

//let lazyViewWithCustomGrouped customComp = Common.lazyView2 (viewWithCustomGrouped customComp)
let lazyViewWithCustomGrouped customComp = Client.ElmishHelpers.prodLazyView (viewWithCustomGrouped customComp)
