module Client.Teabag.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

open Client.Components
open Client.FileUrlHandler
open Client.Teabag.Types
open Services.Dtos
open HtmlProps

let form (teabag: Teabag) (model:Model) dispatch =
    form [AutoComplete Off] [
        (ComboBox.View.viewWithButtons model.brandCmp (BrandCmp >> dispatch))
        Field.div [ ] [
            Label.label [ Label.Option.For "flavour" ] [
                str "Flavour"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: English breakfast"; Input.DefaultValue teabag.flavour; Input.Option.Id "flavour" ]
            ]
        ]
        (ComboBox.View.viewWithoutButtons model.bagtypeCmp (BagtypeCmp >> dispatch))
        (ComboBox.View.viewWithButtons model.countryCmp (CountryCmp >> dispatch))
        Field.div [ ] [
            Label.label [ Label.Option.For "hallmark" ] [
                str "Hallmark"
            ]
            Control.div [ ] [
                Textarea.textarea [ Textarea.Placeholder "Ex: Hallmark"; Textarea.DefaultValue teabag.hallmark; Textarea.Option.Id "hallmark"; Textarea.OnChange (fun ev -> dispatch (HallmarkChanged ev.Value)) ] []
            ]
        ]
        Field.div [ ] [
            Label.label [ Label.Option.For "serialnumber" ] [
                str "Serialnumber"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: 0123456789"; Input.DefaultValue teabag.serialnumber; Input.Option.Id "serialnumber" ]
            ]
        ]
        Field.div [ ] [
            Label.label [ Label.Option.For "serie" ] [
                str "Serie"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: With logo"; Input.DefaultValue teabag.serie; Input.Option.Id "serie" ]
            ]
        ]
    ]

let view (model:Model) (dispatch: Msg -> unit) =
    [
      yield Columns.columns [ Columns.IsMultiline; Columns.IsMobile ] [
          match model.data with
          | Some x ->
              yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull);] [
                  Content.content [] []
                  img [ Src (getUrl x.imageid) ]
              ]
              yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull);] [
                  Content.content [] []
                  Card.card [] [ Card.content [] [ form x model dispatch ] ]
              ]
          | _ ->
              yield Column.column [ ] [
                  yield Content.content [] []
                  yield div [ classList [ "is-active", 1 = 1 ] ] [ str "unloaded" ]
              ]
      ]
    ]
