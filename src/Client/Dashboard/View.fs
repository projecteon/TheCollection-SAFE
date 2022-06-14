module Client.Dashboard.View

open Fable.Core
open Fable.React
open Fable.React.Props
open Fable.FontAwesome

open Client
open Client.Components
open Client.Dashboard.Types

open Fulma

type IHighchartsMap = 
  abstract HighchartPage: props: obj -> ReactElement;

[<ImportAll("../HighchartsMap.js")>]
let mighchartsMap: IHighchartsMap = jsNative

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

let ExpandCollapseIcon currentCount dispatch expandCmd collapseCmd =
  if currentCount = ReChartHelpers.DataCount.Ten then
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch expandCmd) ] ] [Fa.i [ Fa.Solid.ExpandArrowsAlt ] []]
  else
    Icon.icon [ Icon.Props [ OnClick (fun _ -> dispatch collapseCmd) ] ] [Fa.i [ Fable.FontAwesome.Fa.Icon "fas fa-compress-arrows-alt" ] []]

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
                    str (sprintf "Top %i Brands" (int model.displayedByBrands))
                    div [] [
                      ExpandCollapseIcon  model.displayedBrands dispatch ExpandByBrands CollapseByBrands
                    ]
                  ]
                ]
                Panel.Block.div [ ] [
                  renderBarChart model.countByBrands model.displayedByBrands
                ]
              ]
            ]
            Column.column [ Column.Width (Screen.Desktop, Column.IsOneThird); Column.Width (Screen.Mobile, Column.IsFull) ] [
              Panel.panel [] [
                Panel.heading [ ] [ str "Bagtypes distribution"]
                Panel.Block.div [ ] [
                  renderPieChart model.countByBagtypes
                ]
              ]
            ]
            Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
              Panel.panel [] [
                Panel.heading [ ] [
                  div [ Style [ Display DisplayOptions.Flex; JustifyContent "space-between"; AlignItems AlignItemsOptions.Center ]] [
                    str "Inserts per month"
                  ]
                ]
                Panel.Block.div [ ] [
                  PeriodLinehart.view model.countByInserted (lineChartHoverLegend dispatch) model.countByInsertedHoveredKey
                ]
              ]
            ]
            Column.column [ Column.Width (Screen.Desktop, Column.IsTwoThirds); Column.Width (Screen.Mobile, Column.IsFull) ] [
              Panel.panel [] [
                Panel.heading [ ] [
                  div [ Style [ Display DisplayOptions.Flex; JustifyContent "space-between"; AlignItems AlignItemsOptions.Center ]] [
                    str "Per country"
                  ]
                ]
                Panel.Block.div [ ] [
                  //mighchartsMap.HighchartPage {|data = data|}
                   match model.countCountryTLD with
                    | Some x -> mighchartsMap.HighchartPage {|data = x|}
                    | None -> div [ ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Relative; MinWidth "100%"; MinHeight 320]] []
                ]
              ]
            ]
          ]
        ]
      ]
    ]
