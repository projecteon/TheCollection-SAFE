module Index

open Fable.Core.JsInterop
open Feliz
open Feliz.Bulma
open Elmish.HMR // needs to be last

open ClientTypes

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
    Html.div [ prop.className "loginPage"; prop.children (viewPage model dispatch) ] 
  | _ ->
    Bulma.container [
      Client.Components.Navbar.View.view model.CurrentPage (Client.Extensions.canEdit model.User) model.Navbar (NavbarMsg >> dispatch)
      Html.hr [ prop.style [ style.marginTop 0 ] ]
      Html.div (viewPage model dispatch)
    ]
