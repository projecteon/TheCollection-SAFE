module Client.Teabags.View

open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Fulma

open Client.Navigation
open Client.Components.Navbar.View
open Client.FileUrlHandler
open Client.Teabags.Types
open Server.Api.Dtos

open Domain.SharedTypes

let getDisplayValue (model: Model) =
  match model.searchedTerms with
    | Some x -> x
    | _ -> ""

let renderImage(imageId: ImageId) =
  match imageId.Option with
  | Some dbId -> figure [] [ img [ Src (getThumbnailUrl dbId); Style [Width "100%"] ] ]
  | None ->  figure [ Style [ Display DisplayOptions.Flex; JustifyContent "center" ]] [
                Fa.stack [ Fa.Stack.Size Fa.Fa7x; Fa.Stack.Props [Style [Width "100%"; MarginTop "10px"]] ]
                  [ Fa.i [ Fa.Solid.Camera
                           Fa.Stack1x ]
                      [ ]
                    Fa.i [ Fa.Solid.Ban
                           Fa.Stack2x
                           Fa.CustomClass "has-text-danger" ]
                      [ ] ] ]


let resultItem (teabag: Teabag) dispatch =
  Column.column [ Column.Props [Key (teabag.id.Int.ToString())]; Column.Width (Screen.WideScreen, Column.IsOneFifth); Column.Width (Screen.Tablet, Column.IsOneQuarter); Column.Width (Screen.Mobile, Column.IsFull); ][
    Card.card [] [
      Card.image [] [
        renderImage teabag.imageid
      ]
      Card.content [] [
        Heading.p [ Heading.Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is4)] ][ str teabag.brand.description ]
        Heading.p [ Heading.IsSubtitle; Heading.Modifiers [ Modifier.TextSize (Screen.All, TextSize.Is6)] ][ str teabag.flavour ]
        Content.content [ ] [
          div [] [ small [] [ str teabag.serie ] ]
          div [] [ small [] [ str teabag.hallmark ] ]
          div [] [ small [] [ str teabag.serialnumber ] ]
          div [] [ small [] [ str teabag.bagtype.description ] ]
          div [] [ small [] [ str teabag.country.Value.description ] ]
        ]
      ]
      Card.footer [] [
        Card.Footer.a [ Props [ Href (Client.Navigation.toPath (Page.Teabag (teabag.id.Int))); OnClick goToUrl ] ][
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


let CarrigeReturnKeyCode = 13.0
let onKeyDown dispatch (ev: Browser.Types.KeyboardEvent) =
  if ev.keyCode = CarrigeReturnKeyCode then
    ev.preventDefault();
    ev.stopPropagation();
    dispatch Search
  else
    ev |> ignore

let inputElement model dispatch =
  Input.search [
    yield Input.Props [AutoComplete "off"; DOMAttr.OnKeyDown (fun ev -> onKeyDown dispatch ev)]
    yield Input.Option.Id "searchterm"
    yield Input.Disabled model.isLoading
    yield Input.Placeholder (sprintf "Ex: Some searchterm")
    yield Input.ValueOrDefault (getDisplayValue model)
    yield Input.OnChange (fun ev -> dispatch (OnSearchTermChange ev.Value))
    if model.searchError.IsSome then
      yield Input.Color IsDanger
  ]

let searchError model =
  match model.searchError with
  | Some x -> Help.help [ Help.Color IsDanger ] [ str x ]
  | None -> nothing

let resultCount model =
  match model.resultCount with
  | Some x ->  Notification.notification [ Notification.Color IsInfo ]
                [ str <| (sprintf "Result count: %i" <| x) ]
  | None -> nothing

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
    Modal.modal [ Modal.IsActive true; Modal.Props [ Style [ MaxWidth "100vw"] ] ]
      [ Modal.background [ Props [ OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] ] [ ]
        Modal.content [ ] [ img [ Src (getUrl x); Style [ObjectFit "contain"] ] ]
        Modal.close [ Modal.Close.Size IsLarge
                      Modal.Close.OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] [ ] ]
  | None -> nothing

let view (model:Model) (dispatch: Msg -> unit) =
  [
    Container.container [] [
      Section.section [ Section.Props [Style [ PaddingTop 0 ]] ] [
        searchBar model dispatch
        Columns.columns [ Columns.IsMultiline; Columns.IsMobile; Columns.IsCentered ] [
           searchResult model dispatch
        ]
        zoomImage model dispatch
      ]
    ]
  ]
