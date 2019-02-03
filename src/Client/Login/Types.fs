module Client.Login.Types

open Services.Dtos

type Model = {
  userName: string option
  userNameError: string list
  password: string option
  passwordError: string list
  isWorking: bool
  loginError: string option
  hasTriedToLogin: bool 
}

type ExternalMsg =
  | NoOp
  | SignedIn of UserData

type Msg =
  | ChangeUserName of string option
  | ChangePassword of string option
  | Validate
  | ValidateAndLogin 
  | Login
  | LoginSuccess of UserData
  | LoginFailure of exn
