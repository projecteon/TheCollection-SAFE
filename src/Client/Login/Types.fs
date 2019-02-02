module Client.Login.Types

open Services.Dtos

//type LoginViewModel = {
//  Email : string
//  Password : string
//}

type Model = {
  userName: string option
  password: string option
  isWorking: bool
  loginError: string option
}

type ExternalMsg =
  | NoOp
  | SignedIn of UserData

type Msg =
  | ChangeUserName of string option
  | ChangePassword of string option
  | Login
  | LoginSuccess of UserData
  | LoginFailure of exn
