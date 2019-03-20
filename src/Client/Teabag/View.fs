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
    (ComboBox.View.viewWithoutButtons model.brandCmp (BrandCmp >> dispatch))
    Field.div [ ] [
      Label.label [ Label.Option.For "flavour" ] [
        str "Flavour"
      ]
      Control.div [ ] [
          Input.text [ Input.Placeholder "Ex: English breakfast"; Input.Value teabag.flavour; Input.Option.Id "flavour"; Input.OnChange (fun ev -> dispatch (FlavourChanged ev.Value)) ]
      ]
    ]
    (ComboBox.View.viewWithoutButtons model.bagtypeCmp (BagtypeCmp >> dispatch))
    (ComboBox.View.viewWithButtons model.countryCmp (CountryCmp >> dispatch))
    Field.div [ ] [
      Label.label [ Label.Option.For "hallmark" ] [
        str "Hallmark"
      ]
      Control.div [ ] [
        Textarea.textarea [ Textarea.Placeholder "Ex: Hallmark"; Textarea.Value teabag.hallmark; Textarea.Option.Id "hallmark"; Textarea.OnChange (fun ev -> dispatch (HallmarkChanged ev.Value)) ] []
      ]
    ]
    Field.div [ ] [
      Label.label [ Label.Option.For "serialnumber" ] [
        str "Serialnumber"
      ]
      Control.div [ ] [
        Input.text [ Input.Placeholder "Ex: 0123456789"; Input.Value teabag.serialnumber; Input.Option.Id "serialnumber"; Input.OnChange (fun ev -> dispatch (SerialnumberChanged ev.Value)) ]
      ]
    ]
    Field.div [ ] [
      Label.label [ Label.Option.For "serie" ] [
        str "Serie"
      ]
      Control.div [ ] [
        Input.text [ Input.Placeholder "Ex: With logo"; Input.Value teabag.serie; Input.Option.Id "serie"; Input.OnChange (fun ev -> dispatch (SerieChanged ev.Value)) ]
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
