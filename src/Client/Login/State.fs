module Client.Login.State

open Elmish
open Elmish.Browser.Navigation

open Client.Util
open Client.Login.Types
open Client.Navigation
open Services.Dtos
open Domain.SharedTypes
open Domain.Validation

let isValid (model: Model) =
  List.isEmpty model.userNameError
  && List.isEmpty model.passwordError
  && model.userName.String.Length > 0
  && model.password.String.Length > 0

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
  if model.hasTriedToLogin then
    match (EmailValidation.validate model.userName) with
    | Success x -> []
    | Failure y -> [y]
  else
    model.passwordError

let private validatePassword (model: Model) =
  if model.hasTriedToLogin then
    match (PasswordValidation.validate model.password) with
    | Success x -> []
    | Failure y -> [y]
  else
    model.passwordError

let private updateValidationErrorsCmd (model: Model) =
  let userNameErrors = validateUsername model
  let passwordErrors = validatePassword model
  { model with userNameError = userNameErrors; passwordError = passwordErrors; }

let private saveUserData userData =
  (Client.LocalStorage.save "thecollectionUser") userData

let init () =
  Client.LocalStorage.delete "thecollectionUser"
  let initialModel = {
    userName = EmailAddress.Empty
    userNameError = []
    password = Password.Empty
    passwordError = []
    isWorking = false
    loginError = None
    hasTriedToLogin = false
  }
  initialModel, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg>*ExternalMsg =
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
    { model with isWorking = true; loginError = None }, loginCmd { Email = model.userName; Password = model.password}, NoOp
  | LoginSuccess userData ->
    saveUserData userData
    { model with isWorking = false }, ((Navigation.newUrl (toPath Page.Dashboard))), SignedIn userData
  | LoginFailure exn ->
    { model with isWorking = false; loginError = Some "Wrong username or password" }, Cmd.none, NoOp