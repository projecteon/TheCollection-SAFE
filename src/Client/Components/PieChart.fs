module Client.Components.PieChart

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.Recharts
open Fable.Recharts.Props

open Client

module P = Fable.React.Props

type SliceHoveredId = SliceHoveredId of int

//let renderActiveShape props =
//  //printf "%s" (Thoth.Json.Encode.toString 4 props)
//  g [][
//    sector[
//      Cx props?cx
//      Cy props?cy
//      Polar.InnerRadius props?innerRadius
//      Polar.OuterRadius (props?outerRadius + 3.0)
//      Polar.StartAngle props?startAngle
//      Polar.EndAngle props?endAngle
//      P.Fill props?fill
//    ]
//  ]
    
let private cells data =
  data |> Array.mapi (fun i x -> cell [P.Key (i.ToString()); P.Fill (ReChartHelpers.getColor ReChartHelpers.C3Colors i)] []) |> ofArray

let RADIAN = JS.Math.PI / 180.0;
let private renderLabel props =
  if props?percent > 0.05 then
    let radius = props?innerRadius + (props?outerRadius - props?innerRadius) * 0.5;
    let x = props?cx + radius * JS.Math.cos(-props?midAngle * RADIAN);
    let y = props?cy + radius * JS.Math.sin(-props?midAngle * RADIAN);

    text [ Cartesian.X x; Cartesian.Y y; P.Fill "#fff"; Text.TextAnchor (if x > props?cx then "start" else "end"); SVGAttr.Custom("DominantBaseline", "central")] [
      //div [][ str props.name ]
      //div [][ str (sprintf "%g" (props.percent * 100.0)) ]
      str (sprintf "%.1f%%" (props?percent * 100.0))
    ]
  else
    nothing

let private onPieEnter data = // should have sig (evt, activeIndex)
  printf "mouse"
  printf "%s" (Thoth.Json.Encode.toString 4 data)
  
// https://github.com/recharts/recharts/issues/466
let private renderChart data =
    pieChart
        [ ReChartHelpers.margin 15. 20. 5. 0. ] [
          tooltip [][]
          legend [ Legend.IconType ShapeType.Square ][]
          pie [
            Polar.Data data;
            Polar.DataKey "count";
            Polar.NameKey "description";
            Polar.Label renderLabel;
            //Polar.Label true;
            Polar.LabelLine false;
            //Polar.ActiveIndex 1
            //Polar.ActiveShape renderActiveShape
            Polar.OnMouseEnter onPieEnter
            P.Fill "#8884d8"
          ] [
            cells data
          ]
        ]

// https://fable.io/repl/#?code=LYewJgrgNgpgBAEQIYBckDUCWBnCSqYBeqmIAdgFAUgAOMZcAygJ7YozDV0MBiSARrAB0ABRAB3GACcRSAMYBrLvTh9BMIQGEQUmMt4Dh23UIBS2AJJl2U2vtWGNF4DR0p7AUSjAcAC0-efkIASjDy7rQqasKhcr5IUijY9tEasfGJ2KK2NMmRvNDASBSgkLBwwXAAvA7qQgASMFB0Ulmh4SXg0PAi1RXZtMkUAPTDcAAqvvD0YK6Y1nBzCzS2AG6YYPMA5nAoU3BgqEhwSGRgcL4ScCAAZuwMYCBwks+nKLtPNNBQcJjusO8ANoAHgAMn9pPgAHwAXTgAFVgqC+gAiXwoFA0ABcowADEJ8YSsQAOXHEgCsKIoALgIPBNmhcIQAEFxsyAPo8DzjTT1dmMDyadkWABy4w8wXQzORNQAzFRRnAALIAeQQHlBFBQzDoyvATWqFDgxrgAG9EEcsdd+AArOAAXyo2t1SuwOyqRpNAB9VDAUHFkGhPcafTw-XFQthXGRsPBbhUYLgoChgSBbQAaOAwAAeZChCrG8JELPF1L9cBu4d8gaQmmA5wAFABKQ0muA0rZ+mtwZt9FYgHyxs3Btvtv0AQjguij5CHNTD-t8Qkri4RSNpMJHbYBk5t2HIfWn0djQj35GbW5NuhQECkDDPlDbjrbdbAQluIlsg-gnZQ3Yvo4mg2KoKHAUJQr6i6RseMBNpexoNh4Ui2FIYEQQuEaJjBcFlu88x-D2LYem25rdjU-AgNmtJel6cL2pmK4Bkcr4XrhcAQDQhzsD2wBulirpbC2DalE0-H6lAREjkUq68Ts4h-P4bahlW3YALT5oBxoiVADEqcx9asUpkGYTOMbwNOcDqfBcDSXEU6Js8CnWT6IEHEclkQea2mOXsFpoH0XHHPRcCvkIZDkHomlwD6SEoTZboedZbYAEK2OIsZSEIciziAwjSHFslJSa2mZqF4VkHobG6Gc0jJQkmgZO8gWtia-D1Y1tLWQ1CQoA0MCYFs6JwLKuKEl1jVCAA6hsvkACyjQS409UI3aBSOcKAtZ2ZINmOC0poPWJpgpwrUcADSMDMHAKLZiiG2blFzA7XtgL3dZKAgLlKCYDQG6vdZciHdgx1kAA4lIGx-Q9mltahgIHYkR0nTWF1XSizAogA3HAIhCDwmBQD8KIAMTEsScgAIxk3dG7rVQNLrDA4g8eJVpKuJLabFGqB2cRJrBEImyrButJwNZjRIJsZBbEIvgUyLgIC2wqEopNTTZcA8AfbU5TrLg+BECQ5B3VQUXVWAtXtT1cDjuO2mnUGbabiMYzMjQNAUJ+IBbFISDAEIwAKF7Pt+78ZAERxXHwIz4gUF6EHB77-vyXsjAQPw2ByBDNDfQeDY3BADDAIl27lrxfQlhyXI8nyApCqK4qStKcAAFRwBTo24iOgIFw8OA0DzvgeXApiMEIsYoFYNirPgPa94Rw9cwPq4YdWRwtuX8e-Fs4W6DCcFb4nftCCnvjtHI7wok0g6+KpSDu1Sh+2CH-tSIXQA&html=DwCwLgtgNgfAUAAlAUwIYBN4O0iyyoLhgAOAtMgI4CuAlgG4C8A5AMID2AdmMt2QCoBPEsmYIAxlx7cWPAB5gA9OGgBuCSFQAnAM75G1MADMyADmZYcwHeK20SYCVvY6d7OwHNanBDq3jGACJiEh0ALkVFak4SAGsPADpJCEUtNHEwAAEARgA2KIh0VPSwBJJndGoM2i4EiG8EgCsdQJhgRRs7B0tsa1t7R1sXN09vX38gkPDI6LjE5OLUDLJ0dggc-OpCxeXViDKKqrAazjqG5tb2zoH4Pq7B51d3Wi8fPwDgsFJpqJj4pLWinK7HIYGEyB0OQArAl8sDQeCdGdThc2h1+t1EEhrg4nMNnq9xh8phFfnMASk0uJNFowJDsgkAMwJbIFIoAJWQ1O0dORTRaaJxYBgAEgscAoN5Ygg0lAgjowVAISBkPhAkQ0kZJl9QqTxOhOM0klB2NR0EYoNpkBTFKhGqg5IpJQAjHSKZ3UKAQVCKAAMCQA7CzFOJXO7Pd6+aGWghFD0JVKZcg5YEFYIlToVWqNcgtZ9vnqDUbxCazRarTa7Q6nbRXYojFIyKgAO4QtbIRQAFkDCV9IbDDb4LbbeCjrnVcbg7RVGCwwGd7HQggkltcQVQJHIkm4qG8yC0rSxvXQDAQtHQQWT9UzTc36pLqDXgSvtBvG5Il0UJ-ot3di8Ec7KJAsBAA&css=Q
// https://github.com/fable-compiler/fable-react/blob/master/src/Fable.Recharts/Fable.Recharts.fs
// https://github.com/evelinag/fableconf-data-viz
let private renderData data =
  match data with
  | Some x -> x |> renderChart
  | None -> div [ ClassName "pageloader is-white is-active"; Style [Position PositionOptions.Relative; MinWidth "100%"; MinHeight 320]] []

// https://github.com/recharts/recharts/issues/196
let view (data: 'a[] option) =
  responsiveContainer [Responsive.Width "100%"; Responsive.Aspect (4.0/3.0)][
      renderData data
  ]
