module Index

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fulma
open Elmish.HMR // needs to be last

open ClientTypes
open State
open Client.Navigation


// look into https://chriscourses.com/blog/loading-fonts-webpack
importAll "./index.scss"

let viewPage model dispatch =
  match model.PageModel with
  | LoginPageModel m ->
    Client.Login.View.view m (LoginMsg >> dispatch)
  | DashboardPageModel m ->
    Client.Dashboard.View.view m (DashboardMsg >> dispatch)
  | TeabagsPageModel m ->
    Client.Teabags.View.view model.User m (TeabagsMsg >> dispatch)
  | TeabagPageModel m ->
    Client.Teabag.View.view m (TeabagMsg >> dispatch)

/// Constructs the view for the application given the model.
let view model dispatch =
  match model.PageModel with
  | PageModel.LoginPageModel m ->
    div [ Class "loginPage" ] (viewPage model dispatch)
  | _ ->
    Container.container [ ] [
      Client.Components.Navbar.View.view model.CurrentPage (Client.Extensions.canEdit model.User) model.Navbar (NavbarMsg >> dispatch)
      hr [Style [MarginTop 0]]
      div [ ] (viewPage model dispatch)
    ]
