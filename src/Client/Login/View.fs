module Client.Login.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome

open Fulma

open Types
open HtmlProps

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
                form [AutoComplete Off] [
                  Field.div [] [
                    Control.p [ Control.HasIconLeft ] [
                      Input.email [
                        Input.Option.Id "email"
                        Input.Placeholder "your@email.com"
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
                      ]
                      Icon.icon [ Icon.Size IsSmall; Icon.IsLeft ] [
                        Fa.i [ Fa.Solid.Key ] [ ]
                      ]
                    ]
                  ]
                  Button.button [ Button.Color IsInfo; Button.IsFullWidth; Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Login) ][ str "Login"]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
