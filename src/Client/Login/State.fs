module Client.Login.State

open Elmish
open Elmish.Navigation

open Client.Http
open Client.Login.Types
open Client.Navigation
open Server.Api.Dtos
open Domain.SharedTypes
open Domain.Validation

let isValid (model: Model) =
  List.isEmpty model.userNameError
  && List.isEmpty model.passwordError
  && model.userName.String.Length > 0
  && model.password.String.Length > 0

[<Literal>]
let private UserDataKey = "thecollection:Userdata"

let private loadUser () : UserData option =
    let userDecoder = Thoth.Json.Decode.Auto.generateDecoder<UserData>()
    match Client.LocalStorage.load userDecoder UserDataKey with
    | Ok user -> Some user
    | Error _ -> None

let private saveUserData (userData: UserData) =
  (Client.LocalStorage.save UserDataKey) userData

let clearUserData () =
  Client.LocalStorage.delete UserDataKey

let private refreshTokenCmd (model: RefreshTokenViewModel) =
  let userDecoder = Thoth.Json.Decode.Auto.generateDecoder<UserData>()
  Cmd.OfPromise.either
    (postAs "/api/refreshtoken" userDecoder)
    model
    LoginSuccess
    LoginFailure

let private tryRefreshTokenCmd =
  let userData = loadUser();
  match userData with
  | Some x -> Cmd.ofMsg (Msg.RefreshToken {Token = x.Token; RefreshToken = x.RefreshToken})
  | None -> Cmd.none


let private loginCmd (model: LoginViewModel) =
  let userDecoder = Thoth.Json.Decode.Auto.generateDecoder<UserData>()
  Cmd.OfPromise.either
    (postAs "/api/token" userDecoder)
    model
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

let init =
  let initialModel = {
    userName = EmailAddress.Empty
    userNameError = []
    password = Password.Empty
    passwordError = []
    isWorking = false
    loginError = None
    hasTriedToLogin = false
  }
  initialModel, tryRefreshTokenCmd

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
    clearUserData()
    { model with isWorking = false; loginError = Some "Wrong username or password" }, Cmd.none, NoOp
  | Msg.RefreshToken viewModel ->
    { model with isWorking = true; loginError = None }, refreshTokenCmd viewModel, NoOp
