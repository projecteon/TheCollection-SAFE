module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open Fable.Helpers.React
open Fable.Helpers.React.Props

open ClientTypes
open State
open Client.Navigation

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Client.Home.View.view ()

    | TeabagsPageModel m ->
        Client.Teabags.View.view m (TeabagsMsg >> dispatch)

    | TeabagPageModel m ->
        Client.Teabag.View.view m (TeabagMsg >> dispatch)

/// Constructs the view for the application given the model.
let view model dispatch =
    div [Class "container"] [
        Client.Components.Navbar.view()()
        div [ ] (viewPage model dispatch)
    ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.toNavigable urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
