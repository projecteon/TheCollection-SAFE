module State

open Elmish
open Elmish.Browser.Navigation
open Fable.Import

open Server.Api.Dtos
open ClientTypes
open Client.Util
open Client.Navigation

let handleNotFound (model: Model) =
  Browser.console.error("Error parsing url: " + Browser.window.location.href)
  ( model, Navigation.modifyUrl (toPath Page.Login) )

/// The navigation logic of the application given a page identity parsed from the .../#info
/// information in the URL.
let urlUpdate (result: Page option) (model: Model) =
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
    let m = Client.Teabags.State.init(model.User)
    { model with PageModel = TeabagsPageModel m }, Cmd.none

  | Some (Page.Teabag id) ->
    let m, cmd = Client.Teabag.State.init(model.User)
    { model with PageModel = TeabagPageModel m }, Cmd.map TeabagMsg (tryAuthorizationRequest (id |> Some |> cmd) model.User)

  | Some (Page.TeabagNew str) ->
    let m, cmd = Client.Teabag.State.init(model.User)
    { model with PageModel = TeabagPageModel m },  Cmd.map TeabagMsg (tryAuthorizationRequest (cmd None) model.User)

  | Some Page.Dashboard ->
    let m, cmd, cmd2, cmd3 = Client.Dashboard.State.init(model.User)
    { model with PageModel = DashboardPageModel m },  Cmd.batch [   Cmd.map DashboardMsg cmd
                                                                    Cmd.map DashboardMsg cmd2
                                                                    Cmd.map DashboardMsg cmd3 ]


let loadUser () : UserData option =
    let userDecoder = Thoth.Json.Decode.Auto.generateDecoder<UserData>()
    match Client.LocalStorage.load userDecoder "thecollectionUser" with
    | Ok user -> Some user
    | Error _ -> None


let init page =
  let userData = loadUser()
  match userData with
  | Some x ->
    let m, cmd, cmd2, cmd3 = Client.Dashboard.State.init(Some x)
    let model = { User = Some x; PageModel = DashboardPageModel m }
    urlUpdate page model
  | None ->
    let m, cmd = Client.Login.State.init()
    let model = { User = None; PageModel = LoginPageModel m }
    urlUpdate page model

let update msg model =
  match msg, model.PageModel with
  | TeabagsMsg msg, TeabagsPageModel m ->
    let m, cmd = Client.Teabags.State.update msg m
    { model with PageModel = TeabagsPageModel m }, Cmd.map TeabagsMsg cmd
  | TeabagsMsg _, _ ->
    model, Cmd.none
  | TeabagMsg msg, TeabagPageModel m ->
    let m, cmd = Client.Teabag.State.update msg m
    { model with PageModel = TeabagPageModel m }, Cmd.map TeabagMsg cmd
  | TeabagMsg _, _ ->
    model, Cmd.none
  | DashboardMsg msg, DashboardPageModel m ->
    let m, cmd = Client.Dashboard.State.update msg m
    { model with PageModel = DashboardPageModel m }, Cmd.map DashboardMsg cmd
  | DashboardMsg _, _ ->
    model, Cmd.none
  | LoginMsg msg, LoginPageModel m ->
    let m, cmd, exmsg = Client.Login.State.update msg m
    let newModel =
      match exmsg with
      | Client.Login.Types.ExternalMsg.NoOp -> model
      | Client.Login.Types.ExternalMsg.SignedIn userData -> { model with User = Some userData }
    { newModel with PageModel = LoginPageModel m }, Cmd.map LoginMsg cmd
  | LoginMsg _, _ ->
    model, Cmd.none