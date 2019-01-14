module Client.Login.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let view =
    [ Hero.hero [
        Hero.Color IsLight
        Hero.IsFullHeight ] [
        Hero.body [] [
          Container.container [Container.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]] [
            div [ Class "column is-4 is-offset-4 login" ] [
              Heading.h3 [ Heading.Modifiers [ Modifier.TextColor IsGrey ] ] [ str "Login" ]
              Box.box' [] [
                Image.image [ Image.CustomClass "avatar" ] [
                  img [ Src "/svg/teapot.svg" ]
                ]
                form [AutoComplete "off"] [
                  Field.div [] [
                    Control.p [] [
                      Input.email [
                        yield Input.Option.Id "email"
                        yield Input.Placeholder (sprintf "Your Email")
                      ]
                    ]
                  ]
                  Field.div [] [
                    Control.p [] [
                      Input.password [
                        yield Input.Option.Id "password"
                        yield Input.Placeholder (sprintf "Your password")
                      ]
                    ]
                  ]
                  Button.button [ Button.Color IsInfo; Button.IsFullWidth ][ str "Login"]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
