module App

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
importAll "../sass/index.scss"

let viewPage model dispatch =
  match model.PageModel with
  | LoginPageModel m ->
    Client.Login.View.view m (LoginMsg >> dispatch)
  | DashboardPageModel m ->
    Client.Dashboard.View.view m (DashboardMsg >> dispatch)
  | TeabagsPageModel m ->
    Client.Teabags.View.view m (TeabagsMsg >> dispatch)
  | TeabagPageModel m ->
    Client.Teabag.View.view m (TeabagMsg >> dispatch)

/// Constructs the view for the application given the model.
let view model dispatch =
  match model.PageModel with
  | PageModel.LoginPageModel m ->
    div [ Class "loginPage" ] (viewPage model dispatch)
  | _ ->
    Container.container [ ] [
      Client.Components.Navbar.View.view model.CurrentPage model.Navbar (NavbarMsg >> dispatch)
      hr [Style [MarginTop 0]]
      div [ ] (viewPage model dispatch)
    ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

let sessionHandler initial =
  let sub (dispatch: Msg -> unit) =
    Browser.Dom.window.addEventListener(Client.Auth.SessionExpiredEvent, (fun e ->
      (dispatch LogOut)
      e?detail |> Fable.Core.JS.JSON.stringify |> printf "SessionExpiredEvent %s" )
    ) |> ignore
    Browser.Dom.window.addEventListener(Client.Auth.SessionUpdatedEvent, (fun e ->
      (dispatch (e?detail |> unbox |> RefreshUser))
      e?detail |> Fable.Core.JS.JSON.stringify |> printf "SessionUpdatedEvent %s" )
    ) |> ignore
  Cmd.ofSub sub

let withReact =
  if (!!Browser.Dom.window?__INIT_MODEL__)
  then Program.withReactHydrate
  else Program.withReactSynchronous

Program.mkProgram init update view //(lazyView2 view)
|> Program.withSubscription sessionHandler // https://elmish.github.io/elmish/subscriptions.html
|> Program.toNavigable urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
//|> Program.withHMR
#endif
|> withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
