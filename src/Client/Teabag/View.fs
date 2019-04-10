module Client.Teabag.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fulma

open Client.Components
open Client.FileUrlHandler
open Client.Teabag.Types
open Services.Dtos
open HtmlProps
open Client
open Client.Teabag

module R = Fable.Helpers.React


let teabagForm (teabag: Teabag) (model:Model) dispatch =
  form [AutoComplete Off; Disabled model.isWorking] [
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
    Field.div [ Field.IsGrouped; Field.IsGroupedCentered ] [
      Control.div [ ] [
        Button.button [ Button.Color IsPrimary; Button.IsFullWidth; Button.Disabled (State.isValid model.validationErrors = false || model.isWorking || model.data <> model.originaldata); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndSave) ] [
          if model.isWorking then yield Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
          else yield str "Submit"
        ]
      ]
      Control.div [ ] [
        Button.button [ Button.Color IsDanger; Button.IsFullWidth; Button.Disabled (model.isWorking || model.data = model.originaldata); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Reload) ] [ str "Cancel" ]
      ]
    ]
  ]

let fileUploadForm dispatch =
  form [] [
    Field.div [ ]
      [ File.file [ File.IsBoxed; File.Color IsPrimary; File.Size IsLarge; File.IsCentered ]
          [ File.label [ ]
              [ File.input [ GenericOption.Props [ Id "imageinput"; OnChange(fun ev -> ev.target |> Client.Util.convertToFile |> ImageChanged |> dispatch); Accept "image/png, image/jpeg" ] ]
                File.cta [ ]
                  [ File.icon [ ]
                      [ Icon.icon [ ]
                          [ Fa.i [ Fa.Solid.Upload ]
                              [ ] ] ]
                    File.label [ GenericOption.Props [ Props.HtmlFor "imageinput" ] ]
                      [ str "Choose a file..." ] ] ] ] ]
  ]

let uploadFormOrBan (teabag: Teabag) dispatch =
  match teabag.id.Int with
  | 0 ->
    Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [
      Icon.icon [Icon.Size IsLarge; Icon.Modifiers [Modifier.TextColor Color.IsGrey ; Modifier.TextAlignment (Screen.All, TextAlignment.Centered)]] [ Fa.i [ Fa.Solid.Ban; Fa.Size Fa.Fa6x ] [] ]
    ]
  | _ -> fileUploadForm dispatch

let view (model:Model) (dispatch: Msg -> unit) =
  [
    yield Columns.columns [ Columns.IsMultiline; Columns.IsMobile; Columns.IsVCentered ] [
      match model.data with
      | Some x ->
        yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull); ] [
          Content.content [] [
            match x.imageid.Option with
            | Some x -> yield img [ Src (getUrlDbId x) ]
            | None -> yield uploadFormOrBan x dispatch
          ]
        ]
        yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull);] [
          Card.card [] [ Card.content [] [ teabagForm x model dispatch ] ]
        ]
      | _ ->
        yield Column.column [ ] [
          div [ classList [ "is-active", 1 = 1 ] ] [ str "unloaded" ]
        ]
    ]
  ]
