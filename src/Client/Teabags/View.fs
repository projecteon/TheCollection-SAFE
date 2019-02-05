module Client.Teabags.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fulma

open Client.Navigation
open Client.Components.Navbar
open Client.FileUrlHandler
open Client.Teabags.Types
open Services.Dtos

let getDisplayValue (model: Model) =
  match model.searchedTerms with
    | Some x -> x
    | _ -> ""

let resultItem (teabag: Teabag) dispatch =
  Column.column [ Column.Props [Key (teabag.id.Int.ToString())]; Column.Width (Screen.Desktop, Column.IsOneFifth);  Column.Width (Screen.Mobile, Column.IsFull); ][
    Card.card [] [
      Card.image [] [
        figure [] [
            img [ Src (getUrl teabag.imageid) ]
        ]
      ]
      Card.content [] [
        Heading.p [ Heading.Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is4)] ][ str teabag.brand.Value.description ]
        Heading.p [ Heading.IsSubtitle; Heading.Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is6)] ][ str teabag.flavour ]
        Content.content [ ] [
          div [] [ small [] [ str teabag.serie ] ]
          div [] [ small [] [ str teabag.hallmark ] ]
          div [] [ small [] [ str teabag.serialnumber ] ]
          div [] [ small [] [ str teabag.bagtype.Value.description ] ]
          div [] [ small [] [ str teabag.country.Value.description ] ]
        ]
      ]
      Card.footer [] [
        Card.Footer.a [ Props [ Href (Client.Navigation.toPath (Page.Teabag (teabag.id.Int.ToString()))); OnClick goToUrl ] ][
          Icon.icon[ Icon.Size IsSmall; ] [ Fa.i [ Fa.Solid.PencilAlt ][] ]
        ]
        Card.Footer.a [ Props [ OnClick (fun _ -> dispatch (ZoomImageToggle (Some teabag.imageid))) ] ][
          Icon.icon [ Icon.Size IsSmall ] [ Fa.i [ Fa.Solid.Search ][] ]
        ]
      ]
    ]
  ];

let searchResult (model:Model) dispatch =
  model.result
  |> List.map (fun teabag -> resultItem teabag dispatch)
  |> ofList

let inputElement model dispatch =
  Input.text [
    yield Input.Option.Id "searchterm"
    yield Input.Disabled model.isLoading
    yield Input.Placeholder (sprintf "Ex: Some searchterm")
    yield Input.Value (getDisplayValue model)
    yield Input.OnChange (fun ev -> dispatch (OnSearchTermChange ev.Value))
    if model.searchError.IsSome then
      yield Input.Color IsDanger
  ]

let searchError model =
  match model.searchError with
  | Some x -> Help.help [ Help.Color IsDanger ] [ str x ]
  | None -> Fable.Helpers.React.nothing

let resultCount model =
  match model.resultCount with
  | Some x ->  Notification.notification [ Notification.Color IsInfo ]
                [ str <| (sprintf "Resultcount: %i" <| x) ]
  | None -> Fable.Helpers.React.nothing

let searchBar (model:Model) dispatch =
  Field.div [ ] [
    resultCount model
    searchError model
    Field.div [ Field.HasAddons ] [
      Control.p [ Control.IsExpanded; Control.IsLoading model.isLoading ] [
        inputElement model dispatch
      ]
      Control.p [ ] [
        Button.button [
          Button.Disabled (model.searchedTerms.IsNone || model.isLoading)
          Button.Color IsPrimary
          Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Search)
        ] [ Icon.icon [ ] [ Fa.i [ Fa.Solid.Search ][] ] ]
      ]
    ]
  ]

let zoomImage model dispatch =
  match model.zoomImageId with
  | Some x ->
    Modal.modal [ Modal.IsActive true ]
      [ Modal.background [ Props [ OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] ] [ ]
        Modal.content [ ] [ img [ Src (getUrl x) ] ]
        Modal.close [ Modal.Close.Size IsLarge
                      Modal.Close.OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] [ ] ]
  | None -> Fable.Helpers.React.nothing

let view (model:Model) (dispatch: Msg -> unit) =
  [
    div [] [
      searchBar model dispatch
      Columns.columns [ Columns.IsMultiline; Columns.IsMobile ] [
         searchResult model dispatch
      ]
      zoomImage model dispatch
    ]
  ]
