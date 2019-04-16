module Client.Teabag.Brand.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fulma

open Client.Teabag.Brand.Types
open Services.Dtos
open HtmlProps
open Client.ReactHelpers

module R = Fable.Helpers.React

let viewForm (brand: Brand) (model:Model) dispatch  =
  printf "%b %b %b %b" (not (State.isValid model.validationErrors))  model.isWorking  (model.data = model.originaldata) (State.areEqual model.originaldata model.data)
  form [AutoComplete Off; Disabled model.isWorking] [
    Field.div [ ] [
      Label.label [ Label.Option.For "name" ] [
        str "Name"
      ]
      Control.div [ ] [
        Input.text [
          yield Input.Placeholder "Ex: name";
          yield Input.ValueOrDefault brand.name.String;
          yield Input.Option.Id "name";
          yield Input.OnChange (getValue >> NameChanged >> dispatch)
          if (not (Seq.isEmpty model.validationErrors)) then
            yield Input.Color IsDanger
          else if model.doValidation then
            yield Input.Color IsSuccess ]
      ]
    ]
    Field.div [ Field.IsGrouped; Field.IsGroupedCentered ] [
      Control.div [ ] [
        Button.button [ Button.Color IsPrimary; Button.IsFullWidth; Button.Disabled ((not (State.isValid model.validationErrors)) || model.isWorking || model.data = model.originaldata); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch ValidateAndSave) ] [
          if model.isWorking then yield Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
          else yield str "Save"
        ]
      ]
    ]
  ]

let view (model:Model) (dispatch: Msg -> unit) =
  match model.data with
  | Some x ->
    viewForm x model dispatch
  | None ->
    div [ classList [ "is-active", 1 = 1 ] ] [ str "unloaded" ]

let cardModal (model:Model) (dispatch: Msg -> unit) closeDisplay =
  let headerText = if model.data.IsSome && model.data.Value.id.Int > 0 then "Edit Brand" else "New Brand"  
  Modal.modal [ Modal.IsActive true ]
    [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
      Modal.Card.card [ ]
        [ Modal.Card.head [ ]
            [ Modal.Card.title [ ]
                [ str headerText ]
              Delete.delete [ Delete.OnClick closeDisplay ] [ ] ]
          Modal.Card.body [ ]
            [ view model dispatch ] ] ]

