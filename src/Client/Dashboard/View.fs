module Client.Dashboard.View

open Fable.React
open Fable.React.Props
open Fable.FontAwesome

open Client
open Client.Components
open Client.Dashboard.Types

open Fable.C3

open Fulma

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
  | None -> nothing
  | Some chartConfig ->
    match chartConfig.data.``type`` with
    | None -> nothing
    | Some chartType ->
      match chartType with
      | ChartType.Bar -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd BarToPie)) ]][Fa.i [ Fa.Solid.ChartPie ][]])
      | ChartType.Pie -> (Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch (cmd PieToBar)) ]][Fa.i [ Fa.Solid.ChartBar ][]])
      | _ -> nothing

let ExpandCollapseIcon currentCount dispatch expandCmd collapseCmd =
  if currentCount = ReChartHelpers.DataCount.Ten then
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch expandCmd) ] ][Fa.i [ Fa.Solid.ExpandArrowsAlt ][]]
  else
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch collapseCmd) ] ][Fa.i [ Fable.FontAwesome.Fa.Icon "fas fa-compress-arrows-alt" ][]]

let statistics (model:Model) =
  match model.statistics with
  | Some x -> Level.level [ ]
                [ Level.item [ Level.Item.HasTextCentered ]
                    [ div [ ]
                        [ Level.heading [ ]
                            [ str "Total" ]
                          Level.title [ ]
                            [ sprintf "%i" x.TeabagCount |> str ] ] ]
                  Level.item [ Level.Item.HasTextCentered ]
                    [ div [ ]
                        [ Level.heading [ ]
                            [ str "Brands" ]
                          Level.title [ ]
                            [ sprintf "%i" x.BrandCount |> str ] ] ]
                  Level.item [ Level.Item.HasTextCentered ]
                    [ div [ ]
                        [ Level.heading [ ]
                            [ str "Countries" ]
                          Level.title [ ]
                            [ sprintf "%i" x.CountryCount |> str ] ] ] ]
  | None -> nothing

let view (model:Model) dispatch =
    [
      Container.container [] [
        Section.section [ Section.Props [Style [ PaddingTop 0 ]] ] [
          statistics model
          Columns.columns [ Columns.IsCentered; Columns.IsMultiline; Columns.CustomClass "dashboard" ] [
            Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
              Panel.panel [] [
                Panel.heading [ ] [
                  div [ Style [ Display DisplayOptions.Flex; JustifyContent "space-between"; AlignItems AlignItemsOptions.Center ]] [
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
            Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
              Panel.panel [] [
                Panel.heading [ ] [
                  div [ Style [ Display DisplayOptions.Flex; JustifyContent "space-between"; AlignItems AlignItemsOptions.Center ]] [
                    str "Inserted ReChart"
                  ]
                ]
                Panel.block [ ] [
                  PeriodLinehart.view model.countByInserted (lineChartHoverLegend dispatch) model.countByInsertedHoveredKey
                ]
              ]
            ]
          ]
        ]
      ]
    ]
