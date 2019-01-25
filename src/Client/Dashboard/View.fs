module Client.Dashboard.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome

open Client.Components
open Client.Dashboard.Types

open Fable.C3

open Fulma

module P = Fable.Helpers.React.Props
module C3 = Fable.C3.React

let renderBarChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> BarChart.view
  | None -> None |> BarChart.view

let renderPieChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> PieChart.view
  | None -> None |> PieChart.view
  

let TransformationsIcon  (chart: ChartConfig option) dispatch cmd =
  match chart with
  | None -> null
  | Some chartConfig ->
    match chartConfig.data.``type`` with
    | None -> null
    | Some chartType ->
      match chartType with
      | ChartType.Bar -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd BarToPie)) ]][Fa.i [ Fa.Solid.ChartPie ][]])
      | ChartType.Pie -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd PieToBar)) ]][Fa.i [ Fa.Solid.ChartBar ][]])
      | _ -> null
      

let view (model:Model) dispatch =
    [
      br []
      div [ P.Class "columns is-multiline dashboard"] [
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
        div [ P.Class "column is-one-third-desktop is-full-mobile"; ][
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Brands"
                div [][
                  yield (TransformationsIcon model.countBrands dispatch TransformCountByBrand)
                  yield Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (ReloadBrands)) ] ][Fa.i [ Fa.Solid.SyncAlt ][]]
                ]
              ]
            ]
            Panel.block [ ] [
              match model.countBrands with
                | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
                | None -> yield null
            ]
          ]
        ]
        div [ P.Class "column is-one-third-desktop is-full-mobile"; ][
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Bagtypes"
                div [][
                  yield (TransformationsIcon model.countBagtypes dispatch TransformCountByBagtype)
                ]
              ]
            ]
            Panel.block [ ] [
              match model.countBagtypes with
              | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
              | None -> yield null
            ]
          ]
        ]
      ]
      br [] ]
