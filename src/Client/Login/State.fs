module Client.Login.State

open Elmish
open Elmish.Browser.Navigation

open Client.Util
open Client.Login.Types
open Client.Navigation

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

let update (msg:Msg) model : Model*Cmd<Msg> =
  printf "update login"
  match msg with
  | ChangeUserName data ->
    { model with userName = data }, Cmd.none
  | ChangePassword data ->
    { model with password = data }, Cmd.none
  | Login ->
    { model with isWorking = true; loginError = None }, loginCmd { Email = model.userName.Value; Password = model.password.Value} //Cmd.ofMsg (Msg.LoginSuccess { UserName = "Test"; Token = "JWT" })
  | LoginSuccess userData ->
    { model with isWorking = false }, ((Navigation.newUrl (toPath Page.Dashboard)))
  | LoginFailure exn ->
    { model with isWorking = false; loginError = Some "Wrong username or password" }, Cmd.none