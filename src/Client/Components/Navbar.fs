module Client.Components.Navbar

open Elmish.Browser.Navigation
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Helpers.Isomorphic
open Fable.Import

open Client.Navigation
open Services.Dtos

module R = Fable.Helpers.React

type Model = UserData option

type HTMLAttr =
     | [<CompiledName("data-target")>] DataTarget of string
     | [<CompiledName("aria-label")>] AriaLabel of string
     | [<CompiledName("aria-expanded")>] AriaExpanded of bool
     | [<CompiledName("aria-hidden")>] AriaHidden of bool
     interface IHTMLProp

let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let viewLink page description =
  R.a [ Style [ Padding "0 20px" ]
        Class "navbar-item"
        Href (Client.Navigation.toPath page)
        OnClick goToUrl]
      [ R.str description]

let brand =
  div [ Class "navbar-brand" ] [
      yield R.a [ Class "navbar-item"; Href "\\"] [
        yield img [ Src "/svg/teapot.svg"; Style [Width 30; Height 30;]; Alt ""]
      ]
      yield a [
          Role "button"
          Class "navbar-burger burger"
          AriaLabel "menu"
          AriaExpanded false
          DataTarget "navMenu"
        ] [
          yield span [AriaHidden true] []
          yield span [AriaHidden true] []
          yield span [AriaHidden true] []
      ]
  ]

let navMenu =
  div [
      Id "navMenu"
      Class "navbar-menu"
    ] [
      div [ Class "navbar-end" ] [
        yield viewLink Page.Dashboard "Dashboard"
        yield viewLink Page.Teabags "Teabags"
        yield viewLink Page.Login "Login"
      ]
    ]

let inline private clientView () =
    nav [
          Style []
          Class "navbar is-fixed-top"
          Role "navitation"
          AriaLabel "main naviation"
        ] [
          div [Class "container"] [
            yield brand
            yield navMenu
          ]
        ]

let inline private serverView () =
    clientView()

let view() =
    isomorphicView clientView serverView
