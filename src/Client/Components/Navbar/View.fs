module Client.Components.Navbar.View

open Elmish.Navigation
open Fable.Core.JsInterop
open Feliz
open Feliz.Bulma

open Client.Components.Navbar.Types
open Client.Navigation


let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let goToUrlAndToggleBurger page dispatch (e: Browser.Types.MouseEvent) =
  if page = Page.Teabags then
    Client.SessionStorage.delete "thecollection:search"
  dispatch ToggleBurger |> ignore
  goToUrl e |> ignore

  
let viewLink (description: string) page currentPage dispatch =
  Bulma.navbarItem.a [
    if (page = currentPage) then Bulma.navbarItem.isActive
    size.isSize4Mobile
    prop.style [ style.padding (0, 20) ]
    prop.href (Client.Navigation.toPath page)
    prop.onClick (goToUrlAndToggleBurger page dispatch)
    prop.text description
  ]

let brand (model: Model) dispatch =
  Bulma.navbarBrand.div [
    Bulma.navbarItem.a [
      prop.href "\\"
      prop.children [
        Html.img [
          prop.src "/svg/teapot.svg"
          prop.alt ""
          prop.style [
            style.width 30
            style.height 30
          ]
        ]
      ]
    ]
    Html.a [
      prop.role "button"
      prop.className "navbar-burger"
      prop.ariaLabel "menu"
      prop.ariaExpanded model.isBurgerOpen
      prop.custom("data-target", "navMenu")
      prop.onClick (fun _ -> dispatch ToggleBurger)
      prop.children [
        Html.span [ prop.ariaHidden true ]
        Html.span [ prop.ariaHidden true ]
        Html.span [ prop.ariaHidden true ]
      ]
    ]
  ]

let navMenu currentPage canAdd (model: Model) dispatch =
  Bulma.navbarMenu [
    if model.isBurgerOpen then Bulma.navbarMenu.isActive
    prop.children [
      Bulma.navbarEnd.div [
        yield viewLink "Dashboard" Page.Dashboard currentPage dispatch
        yield viewLink "Teabags" Page.Teabags currentPage dispatch
        if canAdd then yield viewLink "New" (Page.Teabag 0) currentPage dispatch
        yield viewLink "Logout" Page.Logout currentPage dispatch
      ]
    ]
  ]

let view currentPage canAdd (model: Model) dispatch =
  Bulma.navbar [
    prop.role "navigation"
    prop.ariaLabel "main naviation"
    prop.className "is-fixed-top"
    prop.children [
      Bulma.container [
        brand model dispatch
        navMenu currentPage canAdd model dispatch
      ]
    ]
  ]