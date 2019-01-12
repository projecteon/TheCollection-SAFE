module Client.Dashboard.View

open Fable.Helpers.React

open Client.Components
open Client.Dashboard.Types

open Fulma

module P = Fable.Helpers.React.Props

let renderBarChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> BarChart.view
  | None -> None |> BarChart.view

let renderPieChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> PieChart.view
  | None -> None |> PieChart.view
  
let view (model:Model) dispatch =
    [
      br []
      div [ P.Class "columns"] [
        div [ P.Class "column is-one-third-desktop is-full-mobile" ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Brands"]
            Panel.block [ ] [
              renderBarChart model.countByBrands
            ]
          ]
        ]
        div [ P.Class "column is-one-third-desktop is-full-mobile" ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Bagtypes"]
            Panel.block [ ] [
              renderPieChart model.countByBagtypes
            ]
          ]
        ]
      ]
      br []
      br [] ]
