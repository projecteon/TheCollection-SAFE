module Client.Teabags.View

open Elmish.Navigation
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Feliz
open Feliz.Bulma

open Client.Navigation
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
  | None ->  figure [ Style [ Display DisplayOptions.Flex; JustifyContent "center" ] ] [
                Fa.stack [ Fa.Stack.Size Fa.Fa7x; Fa.Stack.Props [Style [Width "100%"; MarginTop "10px"]] ] [
                  Fa.i [ Fa.Solid.Camera; Fa.Stack1x ] [ ]
                  Fa.i [ Fa.Solid.Ban; Fa.Stack2x; Fa.CustomClass "has-text-danger" ] [ ]
                ]
              ]


let resultItem (userData: UserData option) (teabag: Teabag) dispatch =
  Bulma.column [
    Bulma.column.isOneFifthWidescreen
    Bulma.column.isOneThirdTablet
    Bulma.column.isFullMobile
    prop.key (teabag.id.Int.ToString())
    prop.children [
      Bulma.card [
        Bulma.cardImage [
          renderImage teabag.imageid
        ]
        Bulma.cardContent [
          prop.style [ style.position.relative ]
          prop.children [
            Bulma.title [ Bulma.title.is4; prop.text teabag.brand.description ]
            Bulma.subtitle [ Bulma.title.is6; prop.text teabag.flavour ]
            Bulma.content [
              Html.div [ Html.small [ prop.text teabag.serie ] ]
              Html.div [ Html.small [ prop.text teabag.hallmark ] ]
              Html.div [ Html.small [ prop.text teabag.serialnumber ] ]
              Html.div [ Html.small [ prop.text teabag.bagtype.description ] ]
              Html.div [ Html.small [ prop.text teabag.country.Value.description ] ]
            ]
            match teabag.archiveNumber with
            | Some x -> div [Style [Position PositionOptions.Absolute; Top -5; Right 5]; ClassName "is-size-7 has-text-weight-semibold is-family-code"] [
              x |> sprintf "#%i" |> str]
            | None -> nothing
          ]
        ]
        Bulma.cardFooter [
          prop.style [ style.borderTopStyle borderStyle.none]
          prop.children [
            if Client.Extensions.canEdit userData then
              Bulma.cardFooterItem.p [ 
                prop.style [ style.borderRightStyle borderStyle.none]
                prop.children [
                  Bulma.button.button [
                    color.isInfo
                    prop.onClick (fun _ -> Navigation.newUrl (Client.Navigation.toPath (Page.Teabag (teabag.id.Int))) |> List.map (fun f -> f ignore) |> ignore )
                    prop.children [
                      Bulma.icon[ Bulma.icon.isSmall; prop.children [ Fa.i [ Fa.Solid.PencilAlt ] [] ] ]
                      Html.span [ prop.text "Edit" ]
                    ]
                  ]
                ]
              ]
            Bulma.cardFooterItem.p [ 
              prop.style [ style.borderRightStyle borderStyle.none]
              prop.children [
                Bulma.button.button [
                  color.isPrimary
                  prop.onClick (fun _ -> dispatch (ZoomImageToggle (Some teabag.imageid)))
                  prop.children [
                    Bulma.icon[ Bulma.icon.isSmall; prop.children [ Fa.i [ Fa.Solid.Search ] [] ] ]
                    Html.span [ prop.text "Image" ]
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
  ];

let searchResult userData (model:Model) dispatch =
  model.result
  |> List.map (fun teabag -> resultItem userData teabag dispatch)
  |> ofList

  
let CarrigeReturnKeyCode = "Enter"
let onKeyDown dispatch (ev: Browser.Types.KeyboardEvent) =
  if ev.code = CarrigeReturnKeyCode then
    ev.preventDefault();
    ev.stopPropagation();
    dispatch Search
  else
    ev |> ignore

let inputElement model dispatch =
  Bulma.input.search [
    prop.id "searchterm"
    prop.autoComplete "off"
    prop.disabled model.isLoading
    prop.placeholder (sprintf "Ex: Some searchterm")
    prop.valueOrDefault (getDisplayValue model)
    prop.onChange (fun value -> dispatch (OnSearchTermChange value))
    prop.onKeyDown (fun ev -> onKeyDown dispatch ev)
    if model.searchError.IsSome then
      color.isDanger
  ]

let searchError model =
  match model.searchError with
  | Some x -> Bulma.help [ color.isDanger; prop.text x ]
  | None -> nothing

let resultCount model =
  match model.resultCount, model.isLoading with
  | Some x, false ->  Bulma.notification [ color.isInfo; prop.text (sprintf "Showing: %i - %i / %i" ((model.page - 1) * 100) (model.result.Length + (model.page - 1) * 100) x) ]
  | Some count, true -> Bulma.notification [ color.isInfo; prop.text (sprintf "Fetching: %i - %i" ((model.page - 1) * 100) ((model.page) * 100)) ]
  | None, _ -> nothing

let searchBar (model:Model) dispatch =
  Bulma.field.div [
    resultCount model
    searchError model
    Bulma.field.div [
      Bulma.field.hasAddons
      prop.children [
        Bulma.control.p [
          Bulma.control.isExpanded
          if model.isLoading then Bulma.control.isLoading 
          prop.children [
            inputElement model dispatch
          ]
        ]
        Bulma.control.p [
          Bulma.button.button [
            prop.disabled (model.searchedTerms.IsNone || model.isLoading)
            color.isPrimary
            prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Search)
            prop.children [ Bulma.icon [ Fa.i [ Fa.Solid.Search ] [] ] ]
          ] 
        ]
      ]
    ]
  ]

let zoomImage model dispatch =
  match model.zoomImageId with
  | Some x ->
      Bulma.modal [
        Bulma.modal.isActive
        prop.style [
          style.maxWidth (length.vw 100)
        ]
        prop.children [
          Bulma.modalBackground [ prop.onClick (fun _ -> dispatch (ZoomImageToggle None)) ]
          Bulma.modalContent [ img [ Src (getUrl x); Style [ObjectFit "contain"] ] ]
          Bulma.modalClose [ prop.onClick (fun _ -> dispatch (ZoomImageToggle None)) ]
        ]
      ]
  | None -> nothing

let view userData (model:Model) (dispatch: Msg -> unit) =
  [
    Bulma.container [
      Bulma.section [
        prop.style [
          style.paddingTop 0
        ]
        prop.children [
          searchBar model dispatch
          match model.resultCount with
          | Some count ->  Client.Components.Pagination.view {currentPage = model.page; pageSize = 100; itemCount = count} (SearchPageCmd >> dispatch)
          | None -> nothing
          Bulma.columns [
            Bulma.columns.isMultiline
            Bulma.columns.isMobile
            Bulma.columns.isCentered
            prop.children [
              searchResult userData model dispatch
            ]
          ]
          zoomImage model dispatch
        ]
      ]
    ]
  ]
