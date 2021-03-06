module Client.Teabag.Bagtype.View

open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Fulma

open Client.Teabag.Bagtype.Types
open Server.Api.Dtos
open HtmlProps
open Client.ReactHelpers
open Client.Teabag.Bagtype

let viewForm (bagtype: Bagtype) (model:Model) dispatch  =
  printf "%b %b %b %b" (not (State.isValid model.validationErrors))  model.isWorking  (model.data = model.originaldata) (State.areEqual model.originaldata model.data)
  form [AutoComplete Off; Disabled model.isWorking] [
    Field.div [ ] [
      Label.label [ Label.Option.For "name" ] [
        str "Name"
      ]
      Control.div [ ] [
        Input.text [
          yield Input.Placeholder "Ex: name";
          yield Input.ValueOrDefault bagtype.name.String;
          yield Input.Option.Id "name";
          yield Input.OnChange (getValue >> NameChanged >> dispatch)
          if (not (model.validationErrors |> State.getNameErrors |> Seq.isEmpty)) then
            yield Input.Color IsDanger
          else if model.doValidation then
            yield Input.Color IsSuccess ]
      ]
      Client.FulmaHelpers.inputError (model.validationErrors |> State.getNameErrors |> List.ofSeq) |> ofList
    ]
    Field.div [ Field.IsGrouped; Field.IsGroupedCentered ] [
      Control.div [ ] [
        Button.button [ Button.Color IsPrimary; Button.IsFullWidth; Button.Disabled (model.data.IsSome && model.data.Value.id.Int = 0); Button.OnClick (fun ev -> ev.preventDefault(); ev.stopPropagation(); dispatch New) ] [
          if model.isWorking then yield Fa.i [ Fa.Solid.CircleNotch; Fa.Spin ] [ ]
          else yield str "New"
        ]
      ]
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
    div [ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Absolute]] []

let headerText (model: Model) =
  if model.data.IsSome && model.data.Value.id.Int > 0 then
    "Edit Bagtype"
  else
    "New Bagtype"

