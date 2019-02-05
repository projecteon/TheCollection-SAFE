module Client.Login.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome

open Fulma

open Types
open HtmlProps
open Client.Login
open Domain.SharedTypes

let private loginBtnContent model =
      if model.isWorking then Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
      else str "Login"

let private loginError model =
  match model.loginError with
  | Some x -> Notification.notification [ Notification.Color IsDanger ] [ str x ]
  | None -> Fable.Helpers.React.nothing

let private inputError errorList =
  errorList
  |> List.map (fun x -> Help.help [ Help.Color IsDanger ] [ str x ])

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
                      yield Input.email [
                        yield Input.Option.Id "email"
                        yield Input.Placeholder "your@email.com"
                        yield Input.Value model.userName.String
                        yield Input.OnChange (fun ev -> dispatch (ChangeUserName (EmailAddress ev.Value)))
                        if (List.isEmpty model.userNameError = false) then
                          yield Input.Color IsDanger
                        else if model.hasTriedToLogin then
                          yield Input.Color IsSuccess
                      ]
                      yield Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ] [
                        Fa.i [ Fa.Solid.Envelope ] [ ]
                      ]
                      for error in inputError model.userNameError ->
                        error
                    ]
                  ]
                  Field.div [] [
                    Control.p [ Control.HasIconLeft ] [
                      Input.password [
                        yield Input.Option.Id "password"
                        yield Input.Placeholder (sprintf "password")
                        yield Input.Value model.password.String
                        yield Input.OnChange (fun ev -> dispatch (ChangePassword (Password ev.Value)))
                        if (List.isEmpty model.passwordError = false) then
                          yield Input.Color IsDanger
                        else if model.hasTriedToLogin then
                          yield Input.Color IsSuccess
                      ]
                      Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ] [
                        Fa.i [ Fa.Solid.Key ] [ ]
                      ]
                    ]
                    (inputError model.passwordError) |> ofList
                  ]
                  Button.button [ Button.Color IsPrimary; Button.IsFullWidth; Button.Disabled (State.isValid model = false); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndLogin) ][
                    loginBtnContent model
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
