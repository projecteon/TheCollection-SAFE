module Client.Login.State

open Elmish
open Elmish.Browser.Navigation

open Client.Util
open Client.Login.Types
open Client.Navigation
open Services.Dtos

let loginCmd (model: LoginViewModel) =
  Cmd.ofPromise
    httpPost ("/api/token", None, model)
    LoginSuccess
    LoginFailure

let init () =
    let initialModel = {
      userName = None
      password = None
      isWorking = false
      loginError = None
    }
    initialModel, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg>*ExternalMsg =
  printf "update login"
  match msg with
  | ChangeUserName data ->
    { model with userName = data }, Cmd.none, NoOp
  | ChangePassword data ->
    { model with password = data }, Cmd.none, NoOp
  | Login ->
    { model with isWorking = true; loginError = None }, loginCmd { Email = EmailAddress model.userName.Value; Password = Password model.password.Value}, NoOp //Cmd.ofMsg (Msg.LoginSuccess { UserName = "Test"; Token = "JWT" })
  | LoginSuccess userData ->
    { model with isWorking = false }, ((Navigation.newUrl (toPath Page.Dashboard))), SignedIn userData
  | LoginFailure exn ->
    { model with isWorking = false; loginError = Some "Wrong username or password" }, Cmd.none, NoOp