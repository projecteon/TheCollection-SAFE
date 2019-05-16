module Client.Login.Types

open Domain.SharedTypes
open Server.Api.Dtos

type Model = {
  userName: EmailAddress
  userNameError: string list
  password: Password
  passwordError: string list
  isWorking: bool
  loginError: string option
  hasTriedToLogin: bool
}

type ExternalMsg =
  | NoOp
  | SignedIn of UserData

type Msg =
  | ChangeUserName of EmailAddress
  | ChangePassword of Password
  | Validate
  | ValidateAndLogin 
  | Login
  | LoginSuccess of UserData
  | LoginFailure of exn
  | RefreshToken of RefreshTokenViewModel
