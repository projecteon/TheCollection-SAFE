module Client.Login.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome

open Fulma

open Types
open HtmlProps

let getDisplayValue (value: string option) =
  match value with
  | Some x -> x
  | _ -> ""

let loginBtnContent model =
      if model.isWorking then Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
      else str "Login"

let loginError model =
  match model.loginError with
  | Some x -> Notification.notification [ Notification.Color IsDanger ] [ str x ]
  | None -> null

let view  (model : Model) (dispatch : Msg -> unit) =
    [ Hero.hero [
        Hero.Color IsLight
        Hero.IsFullHeight ] [
        Hero.body [] [
          Container.container [Container.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]] [
            Column.column [ Column.Width (Screen.All, Column.Is4); Column.Offset (Screen.All, Column.Is4); Column.CustomClass "login" ] [
              Heading.h3 [ Heading.Modifiers [ Modifier.TextColor IsGrey ] ] [ str "The Collection" ]
              Box.box' [] [
                Image.image [ Image.CustomClass "avatar" ] [
                  img [ Src "/svg/teapot.svg" ]
                ]
                loginError model
                form [AutoComplete Off] [
                  Field.div [] [
                    Control.p [ Control.HasIconLeft ] [
                      Input.email [
                        Input.Option.Id "email"
                        Input.Placeholder "your@email.com"
                        Input.Value (getDisplayValue model.userName)
                        Input.OnChange (fun ev -> dispatch (ChangeUserName (Some ev.Value)))
                      ]
                      Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ] [
                        Fa.i [ Fa.Solid.Envelope ] [ ]
                      ]
                    ]
                  ]
                  Field.div [] [
                    Control.p [ Control.HasIconLeft ] [
                      Input.password [
                        Input.Option.Id "password"
                        Input.Placeholder (sprintf "password")
                        Input.Value (getDisplayValue model.password)
                        Input.OnChange (fun ev -> dispatch (ChangePassword (Some ev.Value)))
                      ]
                      Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ] [
                        Fa.i [ Fa.Solid.Key ] [ ]
                      ]
                    ]
                  ]
                  Button.button [ Button.Color IsInfo; Button.IsFullWidth; Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Login) ][
                    loginBtnContent model
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
