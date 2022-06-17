module Client.Login.View

open Fable.React
open Fable.FontAwesome
open Feliz
open Feliz.Bulma

open Types
open Client.Login
open Domain.SharedTypes

let private loginBtnContent model =
      if model.isWorking then Html.i [ prop.className "fa-solid fa-circle-notch fa-spin" ]
      else Html.text "Login"

let private loginError model =
  match model.loginError with
  | Some x -> Bulma.notification [ color.isDanger; prop.text x ] 
  | None -> Html.none

let view  (model : Model) (dispatch : Msg -> unit) =
  [
    Bulma.hero [
      Bulma.color.isLight
      Bulma.hero.isFullHeight
      prop.children [
        Bulma.heroBody [
          Bulma.container [
          text.hasTextCentered
          prop.children [
            Bulma.column [
              column.is4
              column.isOffset4
              prop.className "login"
              prop.children [
                Bulma.title.h3 [
                  Bulma.color.hasTextGrey
                  prop.text "The Collection"
                ]
                Bulma.box [
                  Bulma.image [
                    prop.className "avatar"
                    prop.children [
                      Html.img [ prop.src "/svg/teapot.svg" ]
                    ]
                  ]
                  loginError model
                  Html.form [
                    Bulma.field.div [
                      Bulma.control.p [
                        Bulma.control.hasIconsLeft
                        prop.children [
                          Bulma.input.email [
                            prop.id "email"
                            prop.autoComplete "off"
                            prop.placeholder "your@email.com"
                            prop.valueOrDefault model.userName.String
                            prop.disabled model.isWorking
                            prop.onChange (fun value -> dispatch (ChangeUserName (EmailAddress value)))
                            if (not (List.isEmpty model.userNameError)) then
                              color.isDanger
                            else if model.hasTriedToLogin then
                              color.isSuccess
                          ]
                          Bulma.icon [
                            Bulma.icon.isSmall
                            Bulma.icon.isLeft
                            prop.children [
                              Fa.i [ Fa.Solid.Envelope ] [ ]
                            ]
                          ]
                          (Client.FulmaHelpers.inputError model.userNameError) |> ofList
                        ]
                      ]
                    ]
                    Bulma.field.div [
                      Bulma.control.p [
                        Bulma.control.hasIconsLeft
                        prop.children [
                          Bulma.input.password [
                            prop.id "password"
                            prop.autoComplete "off"
                            prop.placeholder (sprintf "password")
                            prop.valueOrDefault model.password.String
                            prop.disabled model.isWorking
                            prop.onChange (fun value -> dispatch (ChangePassword (Password value)))
                            if (not (List.isEmpty model.passwordError)) then
                              color.isDanger
                            else if model.hasTriedToLogin then
                              color.isSuccess
                          ]
                          Bulma.icon [
                            Bulma.icon.isSmall
                            Bulma.icon.isLeft
                            prop.children [
                              Fa.i [ Fa.Solid.Key ] [ ]
                            ]
                          ]
                          (Client.FulmaHelpers.inputError model.passwordError) |> ofList
                        ]
                      ]
                    ]
                    Bulma.button.button [
                      Bulma.color.isPrimary
                      Bulma.button.isFullWidth
                      prop.disabled ((not (State.isValid model)) || model.isWorking);
                      prop.onClick (fun (ev: Browser.Types.MouseEvent) -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndLogin)
                      prop.children [ loginBtnContent model ]
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
  ]
]