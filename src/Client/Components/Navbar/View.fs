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

let goToUrlAndToggleBurger page dispatch (e: Browser.Types.MouseEvent) =
  if page = Page.Teabags then
    Client.SessionStorage.delete "thecollection:search"
  dispatch ToggleBurger |> ignore
  goToUrl e |> ignore

let viewLink description page currentPage dispatch =
  Navbar.Item.a [ Navbar.Item.IsActive (page = currentPage)
                  Navbar.Item.Modifiers [Modifier.TextSize (Screen.Mobile, TextSize.Is3)]
                  Navbar.Item.Props [ Style [ Padding "0 20px" ]
                                      Href (Client.Navigation.toPath page)
                                      OnClick (goToUrlAndToggleBurger page dispatch) ] ]
                  [ str description]

let brand (model: Model) dispatch =
  Navbar.Brand.div [] [
      Navbar.Item.a [ Navbar.Item.Props [ Href "\\" ]] [
        img [ Src "/svg/teapot.svg"; Style [Width 30; Height 30;]; Alt ""]
      ]
      a [
          Role "button"
          Class "navbar-burger"
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
let navMenu currentPage canAdd (model: Model) dispatch =
  Navbar.menu [
     Navbar.Menu.IsActive model.isBurgerOpen
    ] [
      Navbar.End.div [] [
        yield viewLink "Dashboard" Page.Dashboard currentPage dispatch
        yield viewLink "Teabags" Page.Teabags currentPage dispatch
        if canAdd then yield viewLink "New" (Page.TeabagNew "") currentPage dispatch
        yield viewLink "Logout" Page.Logout currentPage dispatch
      ]
    ]

let view currentPage canAdd (model: Model) dispatch =
    Navbar.navbar [ Navbar.Props [Role "navigation"
                                  AriaLabel "main naviation"]
                    Navbar.CustomClass "is-fixed-top"
        ] [
          Container.container [] [
            brand model dispatch
            navMenu currentPage canAdd model dispatch
          ]
        ]