module Client.Components.ComboBox.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fulma

open Services.Dtos
open Domain.SharedTypes
open Client.Components.ComboBox.Types
open Elmish.React

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

let viewChoices (model: Model) (dispatch : Msg -> unit) =
  match model.HasFocus with
  | true ->
    Dropdown.dropdown [ Dropdown.CustomClass "isDisplayed" ] [
      div [] []
      Dropdown.menu [ ] [
        Dropdown.content [ ] [
          match model.Value with
          | Some x when x.id.Int > 0 ->
            yield Dropdown.Item.a [ Dropdown.Item.Props [ Key (x.id.ToString()); OnMouseDown (fun ev -> dispatch (OnChange x)) ] ] [ str x.description ]
            if model.SearchResult.IsSome then
              yield Dropdown.divider [ ]
          | _ -> ()

          match model.SearchResult with
          | Some results ->
            yield results
            |> List.map (fun refvalue -> Dropdown.Item.a [ Dropdown.Item.Props [ Key (refvalue.id.ToString()); OnMouseDown (fun ev -> dispatch (OnChange refvalue)) ] ] [ str refvalue.description ])
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
    yield Input.Value (getDisplayValue model)
    yield Input.OnChange (fun ev -> dispatch (OnSearchTermChange ev.Value))
    yield Input.Props [
        DOMAttr.OnFocus (fun ev -> dispatch OnFocused)
        DOMAttr.OnBlur (fun ev -> dispatch OnBlur)
    ]
    if model.Value |> hasValue then
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
      viewChoices model dispatch
    ]
  ]

let lazyViewWithoutButtons = Common.lazyView2 lazyViewWithButtons