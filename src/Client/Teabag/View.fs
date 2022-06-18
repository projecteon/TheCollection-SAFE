module Client.Teabag.View

open Fable.FontAwesome
open Feliz
open Feliz.Bulma

open Client.Components
open Client.FileUrlHandler
open Client.Teabag
open Client.Teabag.Types
open Domain.SharedTypes
open Server.Api.Dtos

let customComp (refValue: RefValue) msg dispatch =
  Bulma.control.p [
    Bulma.button.button [
      if refValue.id.Int = 0 then
        color.isSuccess
      else
        color.isInfo
      prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (msg true))
      prop.children [
        if refValue.id.Int = 0 then
          Bulma.icon [ Fa.i [ Fa.Solid.Plus ] [] ]
        else
          Bulma.icon [ Fa.i [ Fa.Solid.EllipsisH ] [] ]
      ]
    ]
  ]

let customCompOptional (refValue: RefValue option) msg dispatch =
  match refValue with
  | Some x -> customComp x msg dispatch
  | None -> customComp EmptyRefValue msg dispatch

let teabagForm (teabag: Teabag) (model:Model) dispatch =
  Html.form [
    prop.autoComplete "off"
    prop.disabled model.isWorking
    prop.children [
      (ComboBox.View.lazyViewWithCustomGrouped (customComp teabag.brand ToggleAddBrandModal dispatch) model.brandCmp (BrandCmp >> dispatch))
      Bulma.field.div [
        Bulma.label [ prop.for' "flavour"; prop.text "Flavour" ]
        Bulma.control.div [
          Bulma.input.text [
            prop.placeholder "Ex: English breakfast";
            prop.valueOrDefault teabag.flavour;
            prop.id "flavour";
            prop.onChange (fun value -> dispatch (FlavourChanged value))
            if teabag.flavour.Length > 0 then
              color.isSuccess
          ]
        ]
      ]
      (ComboBox.View.lazyViewWithCustomGrouped (customComp teabag.bagtype ToggleAddBagtypeModal dispatch) model.bagtypeCmp (BagtypeCmp >> dispatch))
      (ComboBox.View.lazyViewWithCustomGrouped (customCompOptional teabag.country ToggleAddCountryModal dispatch) model.countryCmp (CountryCmp >> dispatch))
      Bulma.field.div [
        Bulma.label [ prop.for' "hallmark"; prop.text "Hallmark" ]
        Bulma.control.div [
          Bulma.textarea [ prop.placeholder "Ex: Hallmark"; prop.valueOrDefault teabag.hallmark; prop.id "hallmark"; prop.onChange (fun value -> dispatch (HallmarkChanged value)) ]
        ]
      ]
      Bulma.field.div [
        Bulma.label [ prop.for' "serialnumber"; prop.text "Serialnumber" ]
        Bulma.control.div [
          Bulma.input.text [ prop.placeholder "Ex: 0123456789"; prop.valueOrDefault teabag.serialnumber; prop.id "serialnumber"; prop.onChange (fun value -> dispatch (SerialnumberChanged value)) ]
        ]
      ]
      Bulma.field.div [
        Bulma.label [ prop.for' "serie"; prop.text "Serie" ]
        Bulma.control.div [
          Bulma.input.text [ prop.placeholder "Ex: With logo"; prop.valueOrDefault teabag.serie; prop.id "serie"; prop.onChange (fun value -> dispatch (SerieChanged value)) ]
        ]
      ]
      Bulma.field.div [
        Bulma.label [ prop.for' "inserted"; prop.text "Inserted" ]
        Bulma.control.div [
          Bulma.input.text [ prop.valueOrDefault (teabag.inserted.ToShortDateString()); prop.id "inserted"; prop.disabled true ]
        ]
      ]
      Bulma.field.div [
        Bulma.field.isGrouped;
        Bulma.field.isGroupedCentered;
        prop.children [
          Bulma.control.div [
            Bulma.button.button [ color.isPrimary; Bulma.button.isFullWidth; prop.disabled ((not (State.isValid model.validationErrors)) || model.isWorking || model.data = model.originaldata); prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndSave); prop.children [
                if model.isWorking then Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
                else Html.text "Save"
              ]
            ]
          ]
          Bulma.control.div [
            Bulma.button.button [ color.isWarning; Bulma.button.isFullWidth; prop.disabled (model.isWorking || model.data = model.originaldata); prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch Reload); prop.text "Undo" ]
          ]
        ]
      ]
    ]
  ]

let fileUploadForm dispatch =
  Html.form [
    Bulma.field.div [
      Bulma.file [ Bulma.file.isBoxed; color.isPrimary; Bulma.file.isLarge; Bulma.file.isCentered; prop.children [
          Bulma.fileLabel.label [
            Bulma.fileInput [ prop.id "imageinput"; prop.onChange(fun (ev: Browser.Types.Event) -> ev.target |> Client.BrowserHelpers.convertToFile |> Upload |> dispatch); prop.accept "image/png, image/jpeg" ] 
            Bulma.fileCta [
              Bulma.fileIcon [
                  Bulma.icon [ Fa.i [ Fa.Solid.Upload ] [ ] ]
              ]
              Bulma.fileLabel.label [ prop.for' "imageinput"; prop.text  "Choose a file..." ]
            ]
          ]
        ]
      ]
    ]
  ]

let uploadFormOrBan (teabag: Teabag) dispatch =
  match teabag.id.Int with
  | 0 ->
    Bulma.content [
      text.hasTextCentered;
      prop.children [
        Bulma.icon [ Bulma.icon.isLarge; color.hasTextGrey; text.hasTextCentered; prop.children [ Fa.i [ Fa.Solid.Ban; Fa.Size Fa.Fa6x ] [] ] ]
      ]
    ]
  | _ -> fileUploadForm dispatch

let viewImage x dispatch =
  Html.div [
    prop.className "block";
    prop.style [ style.position.relative ]
    prop.children [
      Bulma.delete [ Bulma.delete.isLarge; prop.style [ style.position.absolute; style.top (length.rem 0.5); style.right (length.rem 0.5) ]; prop.onClick (fun _ -> None |> Domain.SharedTypes.ImageId |> ImageChanged |> dispatch)]
      Html.img [ prop.src (getImageUrl x) ]
    ]
  ]

let view (model:Model) (dispatch: Msg -> unit) =
  [
    Bulma.columns [
      Bulma.columns.isMultiline
      Bulma.columns.isMobile
      Bulma.columns.isVCentered
      prop.children [
        match model.isWorking with
        | true ->
          Bulma.column [
            Html.div [ prop.className "pageloader is-white is-active"; prop.style [ style.position.absolute; style.height (length.calc "100vh - 60px") ] ]
          ]
        | false ->
            match model.data with
            | Some x ->
              Bulma.column [
                Bulma.column.isHalfDesktop
                Bulma.column.isFullMobile
                prop.children [
                  Bulma.content [
                    match x.imageid.Option with
                    | Some x -> viewImage x dispatch
                    | None -> uploadFormOrBan x dispatch
                  ]
                ]
              ]
              Bulma.column [
                Bulma.column.isHalfDesktop
                Bulma.column.isFullMobile
                prop.children [
                  Bulma.card [
                    Bulma.cardContent [
                      teabagForm x model dispatch
                      match x.archiveNumber with
                      | Some x -> Html.div [ prop.style [ style.position.absolute; style.top 5; style.right 5]; prop.className "is-size-7 has-text-weight-semibold is-family-code"; prop.children [
                                      x |> sprintf "#%i" |> Html.text
                                    ]
                                  ]
                      | None -> Html.none
                    ]
                  ]
                ]
              ]
            | _ ->
              Html.none
        if model.editBagtypeCmp.IsSome then
          Client.Components.ModalCard.view model.editBagtypeCmp.Value (EditBagtypeCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddBagtypeModal false)) Client.Teabag.Bagtype.View.headerText Client.Teabag.Bagtype.View.view
        else
          Html.none
        if model.editBrandCmp.IsSome then
          Client.Components.ModalCard.view model.editBrandCmp.Value (EditBrandCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddBrandModal false)) Client.Teabag.Brand.View.headerText Client.Teabag.Brand.View.view
        else
          Html.none
        if model.editCountryCmp.IsSome then
          Client.Components.ModalCard.view model.editCountryCmp.Value (EditCountryCmp >> dispatch) (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch (ToggleAddCountryModal false)) Client.Teabag.Country.View.headerText Client.Teabag.Country.View.view
        else
          Html.none
      ]
    ]
  ]
