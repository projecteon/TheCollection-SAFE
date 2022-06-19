module Client.Dashboard.View

open Fable.Core
open Fable.React
open Fable.FontAwesome
open Fulma
open Feliz
open Feliz.Bulma

open Client
open Client.Components
open Client.Dashboard.Types


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
    Bulma.icon [ prop.onClick (fun _ -> dispatch expandCmd); prop.children [ Fa.i [ Fa.Solid.ExpandArrowsAlt ] [] ] ]
  else
    Bulma.icon [ prop.onClick (fun _ -> dispatch collapseCmd); prop.children [ Fa.i [ Fa.Solid.CompressArrowsAlt ] [] ] ]

let statistics (model:Model) =
  match model.statistics with
  | Some x -> Bulma.level [
                Bulma.levelItem [
                  text.hasTextCentered
                  prop.children [
                    Html.div [
                      Level.heading [ ] [ str "Total" ]
                      Level.title [ ] [ sprintf "%i" x.TeabagCount |> Html.text ]
                    ]
                  ]
                ]
                Bulma.levelItem [
                  text.hasTextCentered
                  prop.children [
                    Html.div [
                      Level.heading [ ] [ str "Brands" ]
                      Level.title [ ] [ sprintf "%i" x.BrandCount |> Html.text ]
                    ]
                  ]
                ]
                Bulma.levelItem [
                  text.hasTextCentered
                  prop.children [
                    Html.div [
                      Level.heading [ ] [ str "Countries" ]
                      Level.title [ ] [ sprintf "%i" x.CountryCount |> Html.text ]
                    ]
                  ]
                ]
              ]
  | None -> Html.none

let view (model:Model) dispatch =
    [
      Bulma.container [
        Bulma.section [
          prop.style [ style.paddingTop 0]
          prop.children [
            statistics model
            Bulma.columns [
              Bulma.columns.isCentered
              Bulma.columns.isMultiline
              prop.className "dashboard"
              prop.children [
                Bulma.column [
                  Bulma.column.isOneThirdDesktop
                  Bulma.column.isFullMobile
                  prop.children [
                    Bulma.panel [
                      Bulma.panelHeading [
                        Html.div [
                          prop.style [ style.display.flex; style.justifyContent.spaceBetween; style.alignItems.center ]
                          prop.children [
                            Html.text (sprintf "Top %i Brands" (int model.displayedByBrands))
                            Html.div [ ExpandCollapseIcon  model.displayedBrands dispatch ExpandByBrands CollapseByBrands ]
                          ]
                        ]
                      ]
                      Bulma.panelBlock.div [
                        renderBarChart model.countByBrands model.displayedByBrands
                      ]
                    ]
                  ]
                ]
                Bulma.column [
                  Bulma.column.isOneThirdDesktop
                  Bulma.column.isFullMobile
                  prop.children [
                    Bulma.panel [
                      Bulma.panelHeading [ prop.text "Bagtypes distribution"]
                      Bulma.panelBlock.div [
                        renderPieChart model.countByBagtypes
                      ]
                    ]
                  ]
                ]
                Bulma.column [
                  Bulma.column.isTwoThirdsDesktop
                  Bulma.column.isFullMobile
                  prop.children [
                    Bulma.panel [
                      Bulma.panelHeading [
                        Html.div [ prop.style [ style.display.flex; style.justifyContent.spaceBetween; style.alignItems.center ]; prop.text "Inserts per month" ]
                      ]
                      Bulma.panelBlock.div [
                        PeriodLinehart.view model.countByInserted (lineChartHoverLegend dispatch) model.countByInsertedHoveredKey
                      ]
                    ]
                  ]
                ]
                Bulma.column [
                  Bulma.column.isTwoThirdsDesktop
                  Bulma.column.isFullMobile
                  prop.children [
                    Bulma.panel [
                      Bulma.panelHeading [
                        Html.div [ prop.style [ style.display.flex; style.justifyContent.spaceBetween; style.alignItems.center ]; prop.text "Per country" ]
                      ]
                      Bulma.panelBlock.div [
                        //mighchartsMap.HighchartPage {|data = data|}
                         match model.countCountryTLD with
                          | Some x -> mighchartsMap.HighchartPage {|data = x|}
                          | None -> Html.div [ prop.className "pageloader is-white is-active"; prop.style [ style.position.relative; style.minWidth (length.percent 100); style.minHeight 320 ] ]
                      ]
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
