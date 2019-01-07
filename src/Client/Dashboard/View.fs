module Client.Dashboard.View

open Fable.Helpers.React

open Client.Components
open Client.Dashboard.Types

open Fulma

module P = Fable.Helpers.React.Props

let renderChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> BarChart.view
  | None -> None |> BarChart.view

let renderChart2 data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> PieChart.view
  | None -> None |> BarChart.view
  
let view (model:Model) dispatch =
    [
      br []
      div [ P.Class "columns"] [
        div [ P.Class "column is-one-third-desktop is-full-mobile" ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Brands"]
            Panel.block [ ] [
              renderChart model.countByBrands
            ]
          ]
        ]
        div [ P.Class "column is-one-third-desktop is-full-mobile" ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Bagtypes"]
            Panel.block [ ] [
              renderChart2 model.countByBagtypes
            ]
          ]
        ]
      ]
      br []
      br [] ]
