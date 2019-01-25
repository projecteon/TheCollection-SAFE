module State

open Elmish
open Elmish.Browser.Navigation
open Fable.Import

open Services.Dtos
open ClientTypes
open Client.Navigation

let handleNotFound (model: Model) =
  Browser.console.error("Error parsing url: " + Browser.window.location.href)
  ( model, Navigation.modifyUrl (toPath Page.Login) )

/// The navigation logic of the application given a page identity parsed from the .../#info
/// information in the URL.
let urlUpdate (result:Page option) (model: Model) =
  printf "urlUpdate"
  match result with
  | None ->
    handleNotFound model

  | Some Page.Login ->
    let m, cmd = Client.Login.State.init()
    { model with PageModel = LoginPageModel m }, Cmd.none

  | _ when model.User.IsNone ->
    let m, cmd = Client.Login.State.init()
    { model with PageModel = LoginPageModel m }, (Navigation.modifyUrl (toPath Page.Login))

  | Some Page.Teabags ->
    let m = Client.Teabags.State.init()
    { model with PageModel = TeabagsPageModel m }, Cmd.none

  | Some (Page.Teabag id) ->
    let m, cmd = Client.Teabag.State.init()
    { model with PageModel = TeabagPageModel m }, Cmd.map TeabagMsg (cmd id)

  | Some Page.Dashboard ->
    let m, cmd, cmd2 = Client.Dashboard.State.init()
    { model with PageModel = DashboardPageModel m },  Cmd.batch [   Cmd.map DashboardMsg cmd
                                                                    Cmd.map DashboardMsg cmd2 ]

let init page =
  let m, cmd = Client.Login.State.init()
  let model = { User = None; PageModel = LoginPageModel m }
  urlUpdate page model

let update msg model =
  printf "update app"
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
  | DashboardMsg msg, DashboardPageModel m ->
    let m, cmd = Client.Dashboard.State.update msg m
    { model with
        PageModel = DashboardPageModel m }, Cmd.map DashboardMsg cmd
  | DashboardMsg _, _ ->
    model, Cmd.none
  | LoginMsg msg, LoginPageModel m ->
    let m, cmd = Client.Login.State.update msg m
    let user: UserData option = match msg with | Client.Login.Types.Msg.LoginSuccess userData -> Some userData | _ -> None
    { model with
        PageModel = LoginPageModel m; User = user }, Cmd.map LoginMsg cmd
  | LoginMsg _, _ ->
    model, Cmd.none