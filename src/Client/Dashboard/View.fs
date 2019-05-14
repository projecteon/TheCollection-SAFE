module Client.Dashboard.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.FontAwesome

open Client
open Client.Components
open Client.Dashboard.Types

open Fable.C3

open Fulma


module P = Fable.Helpers.React.Props
module C3 = Fable.C3.React

let renderBarChart data (count: ReChartHelpers.DataCount) =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate (int count) |> Some |> BarChart.view
  | None -> None |> BarChart.view

let renderPieChart data =
  match data with
  | Some x -> x |> Array.ofList |> Array.truncate 10 |> Some |> PieChart.view
  | None -> None |> PieChart.view

let lineChartHoverLegend dispatch key =
  dispatch (ToggleCountByInsertedHoveredKey key)
  
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
  
let ExpandCollapseIcon currentCount dispatch expandCmd collapseCmd =
  if currentCount = ReChartHelpers.DataCount.Ten then
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch expandCmd) ] ][Fa.i [ Fa.Solid.ExpandArrowsAlt ][]]
  else
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch collapseCmd) ] ][Fa.i [ Fable.FontAwesome.Fa.Icon "fas fa-compress-arrows-alt" ][]]


let view (model:Model) dispatch =
    [
      br []
      Columns.columns [ Columns.IsMultiline; Columns.CustomClass "dashboard" ] [
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Brands ReChart"
                div [][
                  ExpandCollapseIcon  model.displayedBrands dispatch ExpandByBrands CollapseByBrands
                ]
              ]
            ]
            Panel.block [ ] [
              renderBarChart model.countByBrands model.displayedByBrands
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [ str "Bagtypes ReChart"]
            Panel.block [ ] [
              renderPieChart model.countByBagtypes
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Brands C3"
                div [][
                  TransformationsIcon model.countBrands dispatch TransformCountByBrand
                  ExpandCollapseIcon  model.displayedBrands dispatch ExpandBrands CollapseBrands
                ]
              ]
            ]
            Panel.block [ ] [
              match model.countBrands with
                | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
                | None -> yield  div [ ClassName "pageloader is-white is-active"; Style [Position "relative"; MinWidth "100%"; MinHeight 200]] []
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Bagtypes C3"
                div [][
                  yield (TransformationsIcon model.countBagtypes dispatch TransformCountByBagtype)
                ]
              ]
            ]
            Panel.block [ ] [
              match model.countBagtypes with
              | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
              | None -> yield  div [ ClassName "pageloader is-white is-active"; Style [Position "relative"; MinWidth "100%"; MinHeight 200]] []
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Inserted C3"
                div [][]
              ]
            ]
            Panel.block [ ] [
              match model.countInserted with
              | Some x -> yield (C3.chart { data = x.data; axis = x.axis; height = 320 })
              | None -> yield  div [ ClassName "pageloader is-white is-active"; Style [Position "relative"; MinWidth "100%"; MinHeight 200]] []
            ]
          ]
        ]
        Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
          Panel.panel [] [
            Panel.heading [ ] [
              div [ Style [ Display "flex"; JustifyContent "space-between"; AlignItems "center" ]] [
                str "Inserted ReChart"
              ]
            ]
            Panel.block [ ] [
              PeriodLinehart.view model.countByInserted (lineChartHoverLegend dispatch) model.countByInsertedHoveredKey
            ]
          ]
        ]
      ]
      br []
    ]
