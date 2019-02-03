module Client.Login.State

open Elmish
open Elmish.Browser.Navigation

open Client.Util
open Client.Login.Types
open Client.Navigation
open Services.Dtos
open Services
open Domain.SharedTypes
open Services
open Client.Login
open Services


let isValid (model: Model) =
  List.isEmpty model.userNameError && List.isEmpty model.passwordError

let private loginCmd (model: LoginViewModel) =
  Cmd.ofPromise
    httpPost ("/api/token", None, model)
    LoginSuccess
    LoginFailure

let private tryLoginCmd (model: Model) =
  match (isValid model) with
  | true -> Cmd.ofMsg Login
  | false -> Cmd.none

let private validateUsername (model: Model) =
  let emailAddress = match model.userName with | Some x -> EmailAddress x | None -> EmailAddress ""
  if model.hasTriedToLogin then
    match (EmailValidation.validate emailAddress) with
    | Success x -> []
    | Failure y -> [y]
  else
    model.passwordError

let private validatePassword (model: Model) =
  let password = match model.password with | Some x -> Password x | None -> Password ""
  if model.hasTriedToLogin then
    match (PasswordValidation.validate password) with
    | Success x -> []
    | Failure y -> [y]
  else
    model.passwordError

let private updateValidationErrorsCmd (model: Model) =
  let userNameErrors = validateUsername model
  let passwordErrors = validatePassword model
  { model with userNameError = userNameErrors; passwordError = passwordErrors; }
  
let init () =
    let initialModel = {
      userName = None
      userNameError = []
      password = None
      passwordError = []
      isWorking = false
      loginError = None
      hasTriedToLogin = false
    }
    initialModel, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg>*ExternalMsg =
  printf "update login"
  match msg with
  | ChangeUserName data ->
    { model with userName = data }, Cmd.ofMsg Validate, NoOp
  | ChangePassword data ->
     { model with password = data }, Cmd.ofMsg Validate, NoOp
  | Validate ->
    let validatedModel = updateValidationErrorsCmd model
    validatedModel, Cmd.none, NoOp
  | ValidateAndLogin ->
    let newModel = { model with hasTriedToLogin = true }
    let validatedModel = updateValidationErrorsCmd newModel
    validatedModel, tryLoginCmd validatedModel, NoOp
  | Login ->
    { model with isWorking = true; loginError = None }, loginCmd { Email = EmailAddress model.userName.Value; Password = Password model.password.Value}, NoOp //Cmd.ofMsg (Msg.LoginSuccess { UserName = "Test"; Token = "JWT" })
  | LoginSuccess userData ->
    { model with isWorking = false }, ((Navigation.newUrl (toPath Page.Dashboard))), SignedIn userData
  | LoginFailure exn ->
    { model with isWorking = false; loginError = Some "Wrong username or password" }, Cmd.none, NoOp