module Client.Teabag.View

open Fable.React
open Fable.React.Helpers
open Fable.React.Props
open Fable.FontAwesome
open Fulma

open Client.Components
open Client.FileUrlHandler
open Client.Teabag
open Client.Teabag.Types
open Server.Api.Dtos
open HtmlProps

let customComp (refValue: RefValue) msg dispatch =
  Control.p [ ] [
    Button.button [
      if refValue.id.Int = 0 then
        yield Button.Color IsSuccess
      else
        yield Button.Color IsInfo
      yield Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (msg true)) ] [
        if refValue.id.Int = 0 then
          yield Icon.icon [ ] [ Fa.i [ Fa.Solid.Plus ] [] ]
        else
          yield Icon.icon [ ] [ Fa.i [ Fa.Solid.EllipsisH ] [] ]
    ]
  ]

let customCompOptional (refValue: RefValue option) msg dispatch =
  match refValue with
  | Some x -> customComp x msg dispatch
  | None -> customComp EmptyRefValue msg dispatch

let teabagForm (teabag: Teabag) (model:Model) dispatch =
  form [AutoComplete Off; Disabled model.isWorking] [
    (ComboBox.View.lazyViewWithCustomGrouped (customComp teabag.brand ToggleAddBrandModal dispatch) model.brandCmp (BrandCmp >> dispatch))
    Field.div [ ] [
      Label.label [ Label.Option.For "flavour" ] [
        str "Flavour"
      ]
      Control.div [ ] [
          Input.text [ Input.Placeholder "Ex: English breakfast"; Input.ValueOrDefault teabag.flavour; Input.Option.Id "flavour"; Input.OnChange (fun ev -> dispatch (FlavourChanged ev.Value)) ]
      ]
    ]
    (ComboBox.View.lazyViewWithCustomGrouped (customComp teabag.bagtype ToggleAddBagtypeModal dispatch) model.bagtypeCmp (BagtypeCmp >> dispatch))
    (ComboBox.View.lazyViewWithCustomGrouped (customCompOptional teabag.country ToggleAddCountryModal dispatch) model.countryCmp (CountryCmp >> dispatch))
    Field.div [ ] [
      Label.label [ Label.Option.For "hallmark" ] [
        str "Hallmark"
      ]
      Control.div [ ] [
        Textarea.textarea [ Textarea.Placeholder "Ex: Hallmark"; Textarea.ValueOrDefault teabag.hallmark; Textarea.Option.Id "hallmark"; Textarea.OnChange (fun ev -> dispatch (HallmarkChanged ev.Value)) ] []
      ]
    ]
    Field.div [ ] [
      Label.label [ Label.Option.For "serialnumber" ] [
        str "Serialnumber"
      ]
      Control.div [ ] [
        Input.text [ Input.Placeholder "Ex: 0123456789"; Input.ValueOrDefault teabag.serialnumber; Input.Option.Id "serialnumber"; Input.OnChange (fun ev -> dispatch (SerialnumberChanged ev.Value)) ]
      ]
    ]
    Field.div [ ] [
      Label.label [ Label.Option.For "serie" ] [
        str "Serie"
      ]
      Control.div [ ] [
        Input.text [ Input.Placeholder "Ex: With logo"; Input.ValueOrDefault teabag.serie; Input.Option.Id "serie"; Input.OnChange (fun ev -> dispatch (SerieChanged ev.Value)) ]
      ]
    ]
    Field.div [ Field.IsGrouped; Field.IsGroupedCentered ] [
      Control.div [ ] [
        Button.button [ Button.Color IsPrimary; Button.IsFullWidth; Button.Disabled ((not (State.isValid model.validationErrors)) || model.isWorking || model.data = model.originaldata); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndSave) ] [
          if model.isWorking then yield Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
          else yield str "Save"
        ]
      ]
      Control.div [ ] [
        Button.button [ Button.Color IsWarning; Button.IsFullWidth; Button.Disabled (model.isWorking || model.data = model.originaldata); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Reload) ] [ str "Undo" ]
      ]
    ]
  ]

let fileUploadForm dispatch =
  form [] [
    Field.div [ ]
      [ File.file [ File.IsBoxed; File.Color IsPrimary; File.Size IsLarge; File.IsCentered ]
          [ File.label [ ]
              [ File.input [ GenericOption.Props [ Id "imageinput"; OnChange(fun ev -> ev.target |> Client.BrowserHelpers.convertToFile |> Upload |> dispatch); Accept "image/png, image/jpeg" ] ]
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

let viewImage x dispatch =
  div [ ClassName "block"; Style [ Position PositionOptions.Relative ] ] [
    Delete.delete [ Delete.Size IsLarge; Delete.Props [Style [Position PositionOptions.Absolute; Top ".5rem"; Right ".5rem"];  OnClick (fun ev -> None |> Domain.SharedTypes.ImageId |> ImageChanged |> dispatch)] ] [ ]
    img [ Src (getUrlDbId x) ]
  ]

let view (model:Model) (dispatch: Msg -> unit) =
  [
    yield Columns.columns [ Columns.IsMultiline; Columns.IsMobile; Columns.IsVCentered ] [
      match model.data with
      | Some x ->
        yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull); ] [
          Content.content [] [
            match x.imageid.Option with
            | Some x -> yield viewImage x dispatch
            | None -> yield uploadFormOrBan x dispatch
          ]
        ]
        yield Column.column [Column.Width (Screen.Desktop, Column.IsHalf);  Column.Width (Screen.Mobile, Column.IsFull);] [
          Card.card [] [
            Card.content [] [ teabagForm x model dispatch ]
          ]
        ]
      | _ ->
        yield Column.column [ ] [
          div [ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Absolute; Height "calc(100vh - 60px)"]] []
        ]
      if model.editBagtypeCmp.IsSome then
        yield Client.Components.ModalCard.view model.editBagtypeCmp.Value (EditBagtypeCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddBagtypeModal false)) Client.Teabag.Bagtype.View.headerText Client.Teabag.Bagtype.View.view
      else
        yield nothing
      if model.editBrandCmp.IsSome then
        yield Client.Components.ModalCard.view model.editBrandCmp.Value (EditBrandCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddBrandModal false)) Client.Teabag.Brand.View.headerText Client.Teabag.Brand.View.view
      else
        yield nothing
      if model.editCountryCmp.IsSome then
        yield Client.Components.ModalCard.view model.editCountryCmp.Value (EditCountryCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddCountryModal false)) Client.Teabag.Country.View.headerText Client.Teabag.Country.View.view
      else
        yield nothing
    ]
  ]
