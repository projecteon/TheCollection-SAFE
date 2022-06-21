module Client.Teabag.Brand.View

open Fable.React
open Fable.FontAwesome
open Feliz
open Feliz.Bulma

open Client.Teabag.Brand.Types
open Server.Api.Dtos
open Client.ReactHelpers

let viewForm (brand: Brand) (model:Model) dispatch  =
  Html.form [
    prop.autoComplete "off"
    prop.disabled model.isWorking
    prop.children [
      Bulma.field.div [
        Bulma.label [ prop.for' "name"; prop.text "Name" ]
        Bulma.control.div [
          Bulma.input.text [
            prop.placeholder "Ex: name"
            prop.valueOrDefault brand.name.String
            prop.id "name"
            prop.onChange (getValue >> NameChanged >> dispatch)
            if (not (model.validationErrors |> State.getNameErrors |> Seq.isEmpty)) then
              color.isDanger
            else if model.doValidation then
              color.isSuccess
          ]
        ]
        Client.BulmaHelpers.inputError (model.validationErrors |> State.getNameErrors |> List.ofSeq) |> ofList
      ]
      Bulma.field.div [
        Bulma.field.isGrouped
        Bulma.field.isGroupedCentered
        prop.children [
          Bulma.control.div [ 
            Bulma.button.button [
              color.isPrimary
              button.isFullWidth
              prop.disabled (model.data.IsSome && model.data.Value.id.Int = 0)
              prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch New)
              prop.children [
                if model.isWorking then Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
                else Html.text "New"
              ]
            ]
          ]
          Bulma.control.div [ 
            Bulma.button.button [
              color.isPrimary
              button.isFullWidth
              prop.disabled ((not (State.isValid model.validationErrors)) || model.isWorking || model.data = model.originaldata)
              prop.onClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndSave)
              prop.children [
                if model.isWorking then Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
                else Html.text "Save"
              ]
            ]
          ]
        ]
      ]
    ]
  ]

let view (model:Model) (dispatch: Msg -> unit) =
  match model.data with
  | Some x ->
    viewForm x model dispatch
  | None ->
    Html.div [ prop.className "pageloader is-white is-active"; prop.style [ style.position.absolute ] ]

let headerText (model: Model) =
  if model.data.IsSome && model.data.Value.id.Int > 0 then
    "Edit Brand"
  else
    "New Brand"
