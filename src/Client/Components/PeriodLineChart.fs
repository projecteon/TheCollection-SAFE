module Client.Components.PeriodLinehart

open Fable.Core.JsInterop

open Fable.React
open Fable.React.Props
open Fable.Recharts
open Fable.Recharts.Props
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
        P.Style [Display DisplayOptions.InlineBlock; VerticalAlign "middle"; MarginRight 4]][
          path [P.ClassName "recharts-legend-icon"; P.StrokeWidth 4; P.Stroke color; P.Fill "none"; P.D pathD ][]
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

let inline objectKeys (o: obj) : string seq = upcast Fable.Core.JS.Object.keys(o)

let private lineOpacity hoveredKey currentKey =
  match hoveredKey with
  | Some x when x = currentKey -> 1.0
  | Some x -> 0.2
  | None -> 1.0

let private handleMouseEnter callback evt =
  callback evt?dataKey

let private handleMouseLeave callback evt =
  callback None

let private createChartLines opacityKey (data: string seq) =
  data
  |> Seq.filter (fun x -> fst(System.Int32.TryParse x))
  |> Seq.skip 1
  |> Seq.mapi (fun i x -> line [Cartesian.DataKey x;
                                Cartesian.Dot true;
                                P.Key (i.ToString());
                                P.Stroke (ReChartHelpers.getColor ReChartHelpers.C3Colors i);
                                //P.StrokeWidth 2.
                                P.StrokeOpacity (lineOpacity opacityKey x);] [])
  |> Array.ofSeq
  |> ofArray

type private LegenDataPayload = {
  stroke: string
  name: string
  value: int
}
let private renderToolTipItem data =
  tr [ Style [ Border "1px solid #CCC" ] ][
    td [ Style [ Padding "3px 6px"; FontSize "13px"; ] ][
      legendIcon data.stroke
      //span [Style [Display "inline-block"; Height "10px"; Width "10px"; MarginRight "6px"; BackgroundColor data.stroke ]][]
      str data.name
    ]
    td [ Style [ Padding "3px 6px"; FontSize "13px"; TextAlign TextAlignOptions.Right; BorderLeft "1px dotted #999" ]][ str (sprintf "%i" data.value) ]
  ]

type private LegendData = {
  active: bool
  payload: LegenDataPayload array
}

let private CustomTooltip (legendData: LegendData) =
  if legendData.active then
    let childrenData = legendData.payload |> Array.sortByDescending (fun x -> x.value) |> Array.map (fun x -> renderToolTipItem x)
    table [ P.Style [ BackgroundColor "#fff"; Opacity 0.9; BoxShadow "7px 7px 12px -9px rgb(119,119,119)"; BorderCollapse "collapse"; BorderSpacing 0.0; EmptyCells "show" ] ][
      thead [][
        tr [][
          td [ P.ColSpan 2; P.Style [ Color "#fff"; BackgroundColor "#aaa"; Padding "2px 5px"] ][ str legendData?label ]
        ]
      ]
      tbody [] [
        childrenData |> ofArray
      ]
    ]
  else
    nothing

// https://github.com/recharts/recharts/issues/466
let private renderChart hoverLegend opacityKey data =
  let chartData = transformToChartData data
  let lineElements = chartData |> Seq.head  |> objectKeys |> createChartLines opacityKey
  lineChart
    [ ReChartHelpers.margin 5. 20. 55. 0.
      Chart.Height 320.
      Chart.Data (chartData |> Array.ofSeq) ]
    [ xaxis [Cartesian.DataKey "period"; Cartesian.Interval 0; Fable.Recharts.Props.Text.Angle -45.0; Fable.Recharts.Props.Text.TextAnchor "end"] []
      yaxis [] [] // https://stackoverflow.com/questions/50078787/recharts-set-y-axis-range
      tooltip [ Tooltip.Content CustomTooltip ] []
      legend [ Legend.WrapperStyle (createObj (["bottom" ==> "15px"])); P.OnMouseEnter (handleMouseEnter hoverLegend); P.OnMouseLeave (handleMouseLeave hoverLegend);] []
      //cartesianGrid [] []
      //cartesianGrid [P.StrokeDasharray "3 3"] []
      lineElements
    ]

// https://fable.io/repl/#?code=LYewJgrgNgpgBAEQIYBckDUCWBnCSqYBeqmIAdgFAUgAOMZcAygJ7YozDV0MBiSARrAB0ABRAB3GACcRSAMYBrLvTh9BMIQGEQUmMt4Dh23UIBS2AJJl2U2vtWGNF4DR0p7AUSjAcAC0-efkIASjDy7rQqasKhcr5IUijY9tEasfGJ2KK2NMmRvNDASBSgkLBwwXAAvA7qQgASMFB0Ulmh4SXg0PAi1RXZtMkUAPTDcAAqvvD0YK6Y1nBzCzS2AG6YYPMA5nAoU3BgqEhwSGRgcL4ScCAAZuwMYCBwks+nKLtPNNBQcJjusO8ANoAHgAMn9pPgAHwAXTgAFVgqC+gAiXwoFA0ABcowADEJ8YSsQAOXHEgCsKIoALgIPBNmhcIQAEFxsyAPo8DzjTT1dmMDyadkWABy4w8wXQzORNQAzFRRnAALIAeQQHlBFBQzDoyvATWqFDgxrgAG9EEcsdd+AArOAAXyo2t1SuwOyqRpNAB9VDAUHFkGhPcafTw-XFQthXGRsPBbhUYLgoChgSBbQAaOAwAAeZChCrG8JELPF1L9cBu4d8gaQmmA5wAFABKQ0muA0rZ+mtwZt9FYgHyxs3Btvtv0AQjguij5CHNTD-t8Qkri4RSNpMJHbYBk5t2HIfWn0djQj35GbW5NuhQECkDDPlDbjrbdbAQluIlsg-gnZQ3Yvo4mg2KoKHAUJQr6i6RseMBNpexoNh4Ui2FIYEQQuEaJjBcFlu88x-D2LYem25rdjU-AgNmtJel6cL2pmK4Bkcr4XrhcAQDQhzsD2wBulirpbC2DalE0-H6lAREjkUq68Ts4h-P4bahlW3YALT5oBxoiVADEqcx9asUpkGYTOMbwNOcDqfBcDSXEU6Js8CnWT6IEHEclkQea2mOXsFpoH0XHHPRcCvkIZDkHomlwD6SEoTZboedZbYAEK2OIsZSEIciziAwjSHFslJSa2mZqF4VkHobG6Gc0jJQkmgZO8gWtia-D1Y1tLWQ1CQoA0MCYFs6JwLKuKEl1jVCAA6hsvkACyjQS409UI3aBSOcKAtZ2ZINmOC0poPWJpgpwrUcADSMDMHAKLZiiG2blFzA7XtgL3dZKAgLlKCYDQG6vdZciHdgx1kAA4lIGx-Q9mltahgIHYkR0nTWF1XSizAogA3HAIhCDwmBQD8KIAMTEsScgAIxk3dG7rVQNLrDA4g8eJVpKuJLabFGqB2cRJrBEImyrButJwNZjRIJsZBbEIvgUyLgIC2wqEopNTTZcA8AfbU5TrLg+BECQ5B3VQUXVWAtXtT1cDjuO2mnUGbabiMYzMjQNAUJ+IBbFISDAEIwAKF7Pt+78ZAERxXHwIz4gUF6EHB77-vyXsjAQPw2ByBDNDfQeDY3BADDAIl27lrxfQlhyXI8nyApCqK4qStKcAAFRwBTo24iOgIFw8OA0DzvgeXApiMEIsYoFYNirPgPa94Rw9cwPq4YdWRwtuX8e-Fs4W6DCcFb4nftCCnvjtHI7wok0g6+KpSDu1Sh+2CH-tSIXQA&html=DwCwLgtgNgfAUAAlAUwIYBN4O0iyyoLhgAOAtMgI4CuAlgG4C8A5AMID2AdmMt2QCoBPEsmYIAxlx7cWPAB5gA9OGgBuCSFQAnAM75G1MADMyADmZYcwHeK20SYCVvY6d7OwHNanBDq3jGACJiEh0ALkVFak4SAGsPADpJCEUtNHEwAAEARgA2KIh0VPSwBJJndGoM2i4EiG8EgCsdQJhgRRs7B0tsa1t7R1sXN09vX38gkPDI6LjE5OLUDLJ0dggc-OpCxeXViDKKqrAazjqG5tb2zoH4Pq7B51d3Wi8fPwDgsFJpqJj4pLWinK7HIYGEyB0OQArAl8sDQeCdGdThc2h1+t1EEhrg4nMNnq9xh8phFfnMASk0uJNFowJDsgkAMwJbIFIoAJWQ1O0dORTRaaJxYBgAEgscAoN5Ygg0lAgjowVAISBkPhAkQ0kZJl9QqTxOhOM0klB2NR0EYoNpkBTFKhGqg5IpJQAjHSKZ3UKAQVCKAAMCQA7CzFOJXO7Pd6+aGWghFD0JVKZcg5YEFYIlToVWqNcgtZ9vnqDUbxCazRarTa7Q6nbRXYojFIyKgAO4QtbIRQAFkDCV9IbDDb4LbbeCjrnVcbg7RVGCwwGd7HQggkltcQVQJHIkm4qG8yC0rSxvXQDAQtHQQWT9UzTc36pLqDXgSvtBvG5Il0UJ-ot3di8Ec7KJAsBAA&css=Q
// https://github.com/fable-compiler/fable-react/blob/master/src/Fable.Recharts/Fable.Recharts.fs
// https://github.com/evelinag/fableconf-data-viz
let private renderData data hoverLegend opacityKey =
  match data with
  | Some x -> x |> renderChart hoverLegend opacityKey
  | None -> div [ ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Relative; MinWidth "100%"; MinHeight 320]] []

// https://github.com/recharts/recharts/issues/196
let view (data: CountBy<Moment> list option) hoverLegend opacityKey =
  responsiveContainer [Responsive.Width "100%"][
      renderData data hoverLegend opacityKey
  ]
