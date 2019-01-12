module Client.Teabags.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome

open Client.Navigation
open Client.Components.Navbar
open Client.FileUrlHandler
open Client.Teabags.Types
open Services.Dtos
open Domain.Types

let getDisplayValue (model: Model) =
  match model.searchedTerms with
    | Some x -> x
    | _ -> ""

let resultItem (teabag: Teabag) dispatch =
  div [ Key (teabag.id.ToString()); Class "column is-one-fifth-desktop is-full-mobile" ] [
    yield Card.card [] [
      yield Card.image [] [
        yield figure [] [
            img [ Src (getUrl teabag.imageid) ]
        ]
      ]
      yield Card.content [] [
        yield p [ Class "title is-4" ] [ str teabag.brand.Value.description ]
        yield p [ Class "subtitle is-6" ] [ str teabag.flavour ]
        yield Content.content [ ] [
          div [] [ small [] [ str teabag.serie ] ]
          div [] [ small [] [ str teabag.hallmark ] ]
          div [] [ small [] [ str teabag.serialnumber ] ]
          div [] [ small [] [ str teabag.bagtype.Value.description ] ]
          div [] [ small [] [ str teabag.country.Value.description ] ]
        ]
      ]
      yield Card.footer [] [
        yield a [ Class "card-footer-item   "; Href (Client.Navigation.toPath (Page.Teabag (teabag.id.ToString()))); OnClick goToUrl ] [
          yield Icon.faIcon [ Icon.Size IsSmall; ] [ Fa.icon Fa.I.Pencil ]
        ]
        yield a [ Class "card-footer-item"; OnClick (fun _ -> dispatch (ZoomImageToggle (Some teabag.imageid))) ] [
          yield Icon.faIcon [ Icon.Size IsSmall ] [ Fa.icon Fa.I.Search ]
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
    yield Input.Placeholder (sprintf "Ex: Some searchterm")
    yield Input.Value (getDisplayValue model)
    yield Input.OnChange (fun ev -> dispatch (OnSearchTermChange ev.Value))
    if model.searchError.IsSome then
      yield Input.Color IsDanger
  ]

let searchError model =
  match model.searchError with
  | Some x -> Help.help [ Help.Color IsDanger ] [ str x ]
  | None -> null

let resultCount model =
  match model.resultCount with
  | Some x ->  Notification.notification [ Notification.Color IsInfo ]
                [ str <| (sprintf "Resultcount: %i" <| x) ]
  | None -> null

let searchBar (model:Model) dispatch =
  Field.div [ ] [
    yield resultCount model
    yield searchError model
    yield Field.div [ Field.HasAddons ] [
      Control.p [ Control.IsExpanded; Control.IsLoading model.isLoading ] [
        yield inputElement model dispatch
      ]
      Control.p [ ] [
        Button.button [
          Button.Disabled (model.searchedTerms.IsNone)
          Button.Color IsPrimary
          Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Search)
        ] [ Icon.faIcon [ ] [ Fa.icon Fa.I.Search ] ]
      ]
    ]
  ]

let zoomImage model dispatch =
  match model.zoomImageId with
  | Some x ->
    Modal.modal [ Modal.IsActive true ]
      [ Modal.background [ Props [ OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] ] [ ]
        Modal.content [ ]
          [ img [ Src (getUrl x) ] ]
        Modal.close [ Modal.Close.Size IsLarge
                      Modal.Close.OnClick (fun _ -> dispatch (ZoomImageToggle None)) ] [ ] ]
  | None -> null

let view (model:Model) (dispatch: Msg -> unit) =
  [
    yield div [] [
      yield searchBar model dispatch
      yield div [ Class "columns is-mobile is-multiline" ] [
        yield searchResult model dispatch
      ]
      yield zoomImage model dispatch
    ]
  ]
