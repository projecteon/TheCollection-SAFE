module App

open System
open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core.JsInterop

open ClientTypes
open State
open Client.Navigation

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

// look into https://chriscourses.com/blog/loading-fonts-webpack
//importAll "./index.scss"

let sessionHandler _initial : Sub<Msg> =
  let subscribe (dispatch: Msg -> unit) =
    Browser.Dom.window.addEventListener(Client.Auth.SessionExpiredEvent, (fun e ->
      (dispatch LogOut)
      e?detail |> Fable.Core.JS.JSON.stringify |> printf "SessionExpiredEvent %s" )
    )
    Browser.Dom.window.addEventListener(Client.Auth.SessionUpdatedEvent, (fun e ->
      (dispatch (e?detail |> unbox |> RefreshUser))
      e?detail |> Fable.Core.JS.JSON.stringify |> printf "SessionUpdatedEvent %s" )
    )
    { new IDisposable with member _.Dispose() = () }
  [ [ "sessionHandler" ], subscribe ]

//let withReact =
//  if (!!Browser.Dom.window?__INIT_MODEL__)
//  then Program.withReactHydrate
//  else Program.withReactSynchronous

Program.mkProgram init update Index.view
|> Program.withSubscription sessionHandler // https://elmish.github.io/elmish/subscriptions.html
|> Program.toNavigable urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
