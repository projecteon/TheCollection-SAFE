module Client.Login.Types

open Services.Dtos

type Model = {
  userName: string option
  password: string option
}

type Msg =
| ChangeUserName of string option
| Login
| LoginSuccess of UserData
| LoginFailure of exn
