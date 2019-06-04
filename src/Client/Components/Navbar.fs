module Client.Components.Navbar

open Elmish.Navigation
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.React.Isomorphic
open Fable.Import

open Client.Navigation
open Server.Api.Dtos

type Model = UserData option

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

let viewLink page description =
  a [ Style [ Padding "0 20px" ]
      Class "navbar-item"
      Href (Client.Navigation.toPath page)
      OnClick goToUrl]
      [ str description]

let brand =
  div [ Class "navbar-brand" ] [
      a [ Class "navbar-item"; Href "\\"] [
        img [ Src "/svg/teapot.svg"; Style [Width 30; Height 30;]; Alt ""]
      ]
      a [
          Role "button"
          Class "navbar-burger burger"
          AriaLabel "menu"
          AriaExpanded false
          DataTarget "navMenu"
        ] [
          span [AriaHidden true] []
          span [AriaHidden true] []
          span [AriaHidden true] []
      ]
  ]
  
// https://github.com/Fulma/Fulma/issues/46
// https://github.com/MangelMaxime/fulma-demo/blob/master/src/App.fs#L16-L65
let navMenu =
  div [
      Id "navMenu"
      Class "navbar-menu"
    ] [
      div [ Class "navbar-end" ] [
        viewLink Page.Dashboard "Dashboard"
        viewLink Page.Teabags "Teabags"
        viewLink (Page.TeabagNew "") "New"
        viewLink Page.Logout "Logout"
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
            brand
            navMenu
          ]
        ]

let inline private serverView () =
    clientView()

let view() =
    isomorphicView clientView serverView
