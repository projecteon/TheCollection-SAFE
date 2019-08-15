module Client.Components.Navbar.View


open Elmish.Navigation
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.Import
open Fulma

open Client.Components.Navbar.Types
open Client.Navigation

type HTMLAttr =
     | [<CompiledName("data-target")>] DataTarget of string
     | [<CompiledName("aria-label")>] AriaLabel of string
     | [<CompiledName("aria-expanded")>] AriaExpanded of bool
     | [<CompiledName("aria-hidden")>] AriaHidden of bool
     interface IHTMLProp

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let goToUrlAndToggleBurger dispatch (e: Browser.Types.MouseEvent) =
    dispatch ToggleBurger |> ignore
    goToUrl e |> ignore

let viewLink description page currentPage dispatch =
  printf "%O %O" page currentPage
  a [ Style [ Padding "0 20px" ]
      classList ["navbar-item", true; "is-active", page = currentPage]
      Href (Client.Navigation.toPath page)
      OnClick (goToUrlAndToggleBurger dispatch)]
      [ str description]

let brand (model: Model) dispatch =
  div [ Class "navbar-brand" ] [
      a [ Class "navbar-item"; Href "\\"] [
        img [ Src "/svg/teapot.svg"; Style [Width 30; Height 30;]; Alt ""]
      ]
      a [
          Role "button"
          Class "navbar-burger burger"
          AriaLabel "menu"
          AriaExpanded model.isBurgerOpen
          DataTarget "navMenu"
          OnClick (fun _ -> dispatch ToggleBurger)
        ] [
          span [AriaHidden true] []
          span [AriaHidden true] []
          span [AriaHidden true] []
      ]
  ]

// https://github.com/Fulma/Fulma/issues/46
// https://github.com/MangelMaxime/fulma-demo/blob/master/src/App.fs#L16-L65
let navMenu currentPage (model: Model) dispatch =
  div [
      Id "navMenu"
      classList ["navbar-menu", true; "is-active", model.isBurgerOpen]
    ] [
      div [ Class "navbar-end" ] [
        viewLink "Dashboard" Page.Dashboard currentPage dispatch
        viewLink "Teabags" Page.Teabags currentPage dispatch
        viewLink "New" (Page.TeabagNew "") currentPage dispatch
        viewLink "Logout" Page.Logout currentPage dispatch
      ]
    ]

let view currentPage (model: Model) dispatch =
    nav [
          Style []
          Class "navbar is-fixed-top"
          Role "navitation"
          AriaLabel "main naviation"
        ] [
          Container.container [] [
            brand model dispatch
            navMenu currentPage model dispatch
          ]
        ]