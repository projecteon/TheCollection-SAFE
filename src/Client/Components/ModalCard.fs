module Client.Components.ModalCard

open Fable.React
open Feliz
open Feliz.Bulma

let view (model: 'a) (dispatch: 'b -> unit) closeDisplay (headerText: 'a -> string) view =
  Bulma.modal [
    modal.isActive
    prop.children [
      Bulma.modalBackground [ prop.onClick closeDisplay ]
      Bulma.modalCard [
        Bulma.modalCardHead [
          Bulma.modalCardTitle [ model |> headerText |> str ]
          Bulma.delete [ prop.onClick closeDisplay ]
        ]
        Bulma.modalCardBody [ prop.children [ view model dispatch ] ]
      ]
    ]
  ]

