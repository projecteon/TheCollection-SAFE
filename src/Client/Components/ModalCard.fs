module Client.Components.ModalCard

open Fable.React
open Fable.React.Props
open Fulma

let view (model: 'a) (dispatch: 'b -> unit) closeDisplay (headerText: 'a -> string) view =
  Modal.modal [ Modal.IsActive true ]
    [ Modal.background [ Props [ OnClick closeDisplay ] ] [ ]
      Modal.Card.card [ ]
        [ Modal.Card.head [ ]
            [ Modal.Card.title [ ]
                [ model |> headerText |> str ]
              Delete.delete [ Delete.OnClick closeDisplay ] [ ] ]
          Modal.Card.body [ ]
            [ view model dispatch ] ] ]

