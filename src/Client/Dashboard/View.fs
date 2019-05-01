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

let renderBarChart data count =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate count |> Some |> BarChart.view
  | None -> None |> BarChart.view

let renderPieChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> PieChart.view
  | None -> None |> PieChart.view


let TransformationsIcon  (chart: ChartConfig option) dispatch cmd =
  match chart with
  | None -> Fable.Helpers.React.nothing
  | Some chartConfig ->
    match chartConfig.data.``type`` with
    | None -> Fable.Helpers.React.nothing
    | Some chartType ->
      match chartType with
      | ChartType.Bar -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd BarToPie)) ]][Fa.i [ Fa.Solid.ChartPie ][]])
      | ChartType.Pie -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd PieToBar)) ]][Fa.i [ Fa.Solid.ChartBar ][]])
      | _ -> Fable.Helpers.React.nothing


let view (model:Model) dispatch =
    [
      br []
      Columns.columns [ Columns.IsMultiline; Columns.CustomClass "dashboard" ] [
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Brands"
                div [][
                  if model.displayedBrands = 10 then
                    yield Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (ExpandBrands)) ] ][Fa.i [ Fa.Solid.ExpandArrowsAlt ][]]
                  else
                    yield Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (CollapseBrands)) ] ][Fa.i [ Fa.Solid.Compress ][]]
                ]
              ]
            ]
            Panel.block [ ] [
              renderBarChart model.countByBrands model.displayedBrands
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Bagtypes"]
            Panel.block [ ] [
              renderPieChart model.countByBagtypes
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Brands"
                div [][
                  yield (TransformationsIcon model.countBrands dispatch TransformCountByBrand)
                ]
              ]
            ]
            Panel.block [ ] [
              match model.countBrands with
                | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
                | None -> yield Fable.Helpers.React.nothing
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
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
              | None -> yield Fable.Helpers.React.nothing
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Inserted"
                div [][]
              ]
            ]
            Panel.block [ ] [
              match model.countInserted with
              | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
              | None -> yield Fable.Helpers.React.nothing
            ]
          ]
        ]
      ]
      br []
    ]
