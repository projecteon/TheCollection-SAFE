module Client.Components.PeriodLinehart

open Fable.Core.JsInterop

open Feliz
open Feliz.Recharts
// Fable.React opened last so its svg/path/text element functions win over the
// Feliz / Feliz.Recharts prop-module types of the same name.
open Fable.React
open Fable.React.Props
open Fable.Import.Moment.Moment

open Client
open Client.Extensions
open Domain.SharedTypes

module P = Fable.React.Props

let SIZE = 32.0
let halfSize = SIZE / 2.0;
let sixthSize = SIZE / 6.0;
let thirdSize = SIZE / 3.0;

let pathD = sprintf "M0,%fh%f A%f,%f,0,1,1,%f,%f H%fM%f,%f A%f,%f,0,1,1,%f,%f" halfSize thirdSize sixthSize sixthSize (2.0*thirdSize) halfSize SIZE (2.0*thirdSize) halfSize sixthSize sixthSize thirdSize halfSize

let private legendIcon color =
  svg [ P.ClassName "recharts-surface"
        P.Width 14
        P.Height 14
        P.ViewBox (sprintf "0 0 %f %f" SIZE SIZE)
        P.Version "1.1"
        P.Style [Display DisplayOptions.InlineBlock; VerticalAlign "middle"; MarginRight 4]] [
          path [P.ClassName "recharts-legend-icon"; P.StrokeWidth 4; P.Stroke color; P.Fill "none"; P.D pathD ] []
        ]

let private createXAxisLabels (model: CountBy<Moment> list) =
  model
  |> Seq.ofList
  |> Seq.take 12
  |> Seq.map (fun x -> x.description.clone().year(1900.0).format("MMMM"))

let private groupDataPerYear (model: CountBy<Moment> list) =
  model
  |> Seq.ofList
  |> (Seq.split 12)

let monthsInYear = [| for i in 1 .. 12 -> i |]
let private transformData (yearGroupedData: CountBy<Moment> list list) =
  monthsInYear
  |> Array.map (fun x -> yearGroupedData |> List.map (fun y -> ([y.[x - 1].description.year().ToString() ==> y.[x - 1].count])) |> List.concat)
  |> List.ofArray

let private transformToChartData (model: CountBy<Moment> list) =
  let yearData = model |> groupDataPerYear |> transformData
  model
  |> createXAxisLabels
  |> Seq.mapi (fun i x -> createObj (["period" ==> x] |> List.append yearData.[i]))

let inline objectKeys (o: obj) : string seq = upcast Fable.Core.JS.Constructors.Object.keys(o)

let private lineOpacity hoveredKey currentKey =
  match hoveredKey with
  | Some x when x = currentKey -> 1.0
  | Some x -> 0.2
  | None -> 1.0

let private toKey (o: obj) : string option =
  match o with
  | null -> None
  | v -> Some (unbox<string> v)

// Recharts legend payload exposes the series id under `dataKey`; some versions
// only populate `value`. Fall back so the highlight works either way.
let private legendKey (e: obj) : string option =
  match toKey e?dataKey with
  | Some k -> Some k
  | None -> toKey e?value

let private createChartLines opacityKey (data: string seq) =
  data
  |> Seq.filter (fun x -> fst(System.Int32.TryParse x))
  |> Seq.skip 1
  |> Seq.mapi (fun i x ->
      // line/legend prop-modules are fully qualified: bare `line`/`legend` resolve
      // to the Fable.React <line>/<legend> elements once Fable.React is opened.
      Recharts.line [
        Interop.mkLineAttr "key" x   // React list key; silences "unique key" warning
        Feliz.Recharts.line.dataKey x
        Feliz.Recharts.line.dot true
        Feliz.Recharts.line.stroke (ReChartHelpers.getColor ReChartHelpers.C3Colors i)
        // Feliz.Recharts 5.1.0 maps line.strokeOpacity to the hyphenated
        // "stroke-opacity", which Recharts' camelCase prop whitelist drops. Set the
        // camelCase prop directly so the hover dim/highlight actually applies.
        Interop.mkLineAttr "strokeOpacity" (lineOpacity opacityKey x)
      ])
  |> List.ofSeq

type private LegenDataPayload = {
  stroke: string
  name: string
  value: int
}
let private renderToolTipItem data =
  tr [ Key data.name; Style [ Border "1px solid #CCC" ] ] [
    td [ Style [ Padding "3px 6px"; FontSize "13px"; ] ] [
      legendIcon data.stroke
      str data.name
    ]
    td [ Style [ Padding "3px 6px"; FontSize "13px"; TextAlign TextAlignOptions.Right; BorderLeft "1px dotted #999" ]] [ str (sprintf "%i" data.value) ]
  ]

// recharts calls this with the tooltip props object
let private CustomTooltip (tooltipData: obj) =
  if tooltipData?active then
    let payload : LegenDataPayload array = tooltipData?payload
    let childrenData = payload |> Array.sortByDescending (fun x -> x.value) |> Array.map (fun x -> renderToolTipItem x)
    table [ P.Style [ BackgroundColor "#fff"; Opacity 0.9; BoxShadow "7px 7px 12px -9px rgb(119,119,119)"; BorderCollapse "collapse"; BorderSpacing 0.0; EmptyCells "show" ] ] [
      thead [] [
        tr [] [
          td [ P.ColSpan 2; P.Style [ Color "#fff"; BackgroundColor "#aaa"; Padding "2px 5px"] ] [ str tooltipData?label ]
        ]
      ]
      tbody [] [
        childrenData |> ofArray
      ]
    ]
  else
    nothing

let private renderChart hoveredKey setHoveredKey data =
  let chartData = transformToChartData data
  let lineElements = chartData |> Seq.head |> objectKeys |> createChartLines hoveredKey
  Recharts.lineChart [
    lineChart.data (chartData |> Array.ofSeq)
    lineChart.margin(top = 5, right = 20, bottom = 55, left = 0)
    lineChart.children (
      [ Recharts.xAxis [ Interop.mkXAxisAttr "key" "xaxis"; xAxis.dataKey "period"; xAxis.interval 0; xAxis.angle (-45.0); xAxis.textAnchor.textAnchorEnd ]
        Recharts.yAxis [ Interop.mkYAxisAttr "key" "yaxis" ]
        Recharts.tooltip [ Interop.mkTooltipAttr "key" "tooltip"; tooltip.content (fun props -> CustomTooltip (box props)) ]
        Recharts.legend [
          Interop.mkLegendAttr "key" "legend"
          Feliz.Recharts.legend.onMouseEnter (fun e -> setHoveredKey (legendKey (box e)))
          Feliz.Recharts.legend.onMouseLeave (fun () -> setHoveredKey None)
        ] ]
      @ lineElements)
  ]

let private renderData hoveredKey setHoveredKey data =
  match data with
  | Some x -> renderChart hoveredKey setHoveredKey x
  | None -> div [ ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Relative; MinWidth "100%"; MinHeight 320]] []

// Legend-hover highlight lives in component-local state so a purely-visual hover
// never dispatches Elmish msgs / re-renders the rest of the dashboard.
[<ReactComponent>]
let View (data: CountBy<Moment> list option) =
  let hoveredKey, setHoveredKey = React.useState(fun () -> (None: string option))
  Recharts.responsiveContainer [
    responsiveContainer.width (length.percent 100)
    responsiveContainer.height 320
    responsiveContainer.chart (renderData hoveredKey setHoveredKey data)
  ]
