module Client.Components.BarChart

open Fable.React
open Fable.React.Props
open Feliz
open Feliz.Recharts

open Client

let private cells data =
  data
  |> Array.mapi (fun i _ ->
      Recharts.cell [ cell.key (string i); cell.fill (ReChartHelpers.getColor ReChartHelpers.C3Colors i) ])
  |> List.ofArray

let private renderChart (data: 'a[]) =
  Recharts.barChart [
    barChart.data data
    barChart.margin(top = 5, right = 20, bottom = 55, left = 0)
    barChart.children [
      Recharts.xAxis [ xAxis.dataKey "description"; xAxis.interval 0; xAxis.angle (-45.0); xAxis.textAnchor.textAnchorEnd ]
      Recharts.yAxis [ ]
      Recharts.tooltip [ ]
      Recharts.cartesianGrid [ cartesianGrid.strokeDasharray(3, 3) ]
      Recharts.bar [
        bar.dataKey "count"
        bar.stackId "a"
        bar.fill "#8884d8"
        bar.children (cells data)
      ]
    ]
  ]

let private renderData data =
  match data with
  | Some x -> renderChart x
  | None -> div [ ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Relative; MinWidth "100%"; MinHeight 320]] []

let view (data: 'a[] option) =
  Recharts.responsiveContainer [
    responsiveContainer.width (length.percent 100)
    responsiveContainer.aspect (4.0 / 3.0)
    responsiveContainer.chart (renderData data)
  ]
