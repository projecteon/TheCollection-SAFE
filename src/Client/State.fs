module State

open Elmish
open Elmish.Browser.Navigation
open Fable.Import

open ClientTypes
open Client.Navigation

let handleNotFound (model: Model) =
  Browser.console.error("Error parsing url: " + Browser.window.location.href)
  ( model, Navigation.modifyUrl (toPath Page.Home) )

/// The navigation logic of the application given a page identity parsed from the .../#info
/// information in the URL.
let urlUpdate (result:Page option) (model: Model) =
  match result with
  | None ->
    handleNotFound model

  | Some Page.Teabags ->
    let m = Client.Teabags.State.init()
    { model with PageModel = TeabagsPageModel m }, Cmd.none

  | Some (Page.Teabag id) ->
    let m, cmd = Client.Teabag.State.init()
    { model with PageModel = TeabagPageModel m }, Cmd.map TeabagMsg (cmd id)

  | Some Page.Home ->
    { model with PageModel = HomePageModel }, Cmd.none

let init page =
  let model =
    { User = None
      PageModel = HomePageModel }
  urlUpdate page model

let update msg model =
  match msg, model.PageModel with
  | TeabagsMsg msg, TeabagsPageModel m ->
    let m, cmd = Client.Teabags.State.update msg m
    { model with
        PageModel = TeabagsPageModel m }, Cmd.map TeabagsMsg cmd
  | TeabagsMsg _, _ ->
    model, Cmd.none
  | TeabagMsg msg, TeabagPageModel m ->
    let m, cmd = Client.Teabag.State.update msg m
    { model with
        PageModel = TeabagPageModel m }, Cmd.map TeabagMsg cmd
  | TeabagMsg _, _ ->
    model, Cmd.none