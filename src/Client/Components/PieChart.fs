module Client.Components.PieChart

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.Recharts
// Fable.React opened last so its svg/text element functions win over the
// Feliz / Feliz.Recharts prop-module types of the same name.
open Fable.React
open Fable.React.Props

open Client

type SliceHoveredId = SliceHoveredId of int

let private cells data =
  data
  |> Array.mapi (fun i _ ->
      Recharts.cell [ cell.key (string i); cell.fill (ReChartHelpers.getColor ReChartHelpers.C3Colors i) ])
  |> List.ofArray

let RADIAN = JS.Math.PI / 180.0

// recharts calls this with the slice label props object
let private renderLabel (props: obj) : ReactElement =
  if props?percent > 0.05 then
    let radius = props?innerRadius + (props?outerRadius - props?innerRadius) * 0.5
    let x = props?cx + radius * JS.Math.cos(-props?midAngle * RADIAN)
    let y = props?cy + radius * JS.Math.sin(-props?midAngle * RADIAN)
    text [ SVGAttr.X x
           SVGAttr.Y y
           SVGAttr.Fill "#fff"
           SVGAttr.TextAnchor (if x > props?cx then "start" else "end")
           SVGAttr.Custom("dominantBaseline", "central") ] [
      str (sprintf "%.1f%%" (props?percent * 100.0))
    ]
  else
    nothing

let private renderChart (data: 'a[]) =
  Recharts.pieChart [
    pieChart.margin(top = 15, right = 20, bottom = 5, left = 0)
    pieChart.children [
      Recharts.tooltip [ Interop.mkTooltipAttr "key" "tooltip" ]
      Recharts.legend [ Interop.mkLegendAttr "key" "legend" ]
      Recharts.pie [
        Interop.mkPieAttr "key" "pie"
        pie.data data
        pie.dataKey "count"
        pie.nameKey "description"
        pie.label (fun props -> renderLabel (box props))
        pie.labelLine false
        pie.fill "#8884d8"
        pie.children (cells data)
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
