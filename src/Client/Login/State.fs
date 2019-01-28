module Client.Login.State

open Elmish
open Elmish.Browser.Navigation

open Client.Login.Types
open Client.Navigation
open Services.Dtos

let init () =
    let initialModel = {
      userName = None
      password = None
    }
    initialModel, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg> =
  printf "update login"
  match msg with
  | ChangeUserName data ->
    { model with userName = data }, Cmd.none
  | Login ->
    model, Cmd.ofMsg (Msg.LoginSuccess { UserName = "Test"; Token = "JWT" })
  | LoginSuccess userData ->
    model, ((Navigation.newUrl (toPath Page.Dashboard)))
  | LoginFailure exn ->
    model, Cmd.none
