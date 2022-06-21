module Client.Components.PieChart

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.Recharts

open Client
open Client.Recharts
    
let private cells data =
  data |> Array.mapi (fun i x -> Recharts.cell [ cell.key (i.ToString()); cell.fill (ColorHelpers.getColor ColorHelpers.C3Colors i)] )

let RADIAN = JS.Math.PI / 180.0;
let private renderLabel props =
  if props?percent > 0.05 then
    let radius = props?innerRadius + (props?outerRadius - props?innerRadius) * 0.5;
    let x = props?cx + radius * JS.Math.cos(-props?midAngle * RADIAN);
    let y = props?cy + radius * JS.Math.sin(-props?midAngle * RADIAN);

    Svg.text [
      svg.x x
      svg.y y
      svg.fill color.white
      if x > props?cx then svg.textAnchor.startOfText else svg.textAnchor.endOfText
      svg.dominantBaseline.central
      svg.text (sprintf "%.1f%%" (props?percent * 100.0))
    ]
  else
    Html.none

let private onPieEnter data index event = // should have sig (data, activeIndex, event)
  //printf "%s %i" (Thoth.Json.Encode.toString 4 data) index
  printf "%i" index
  
// https://github.com/recharts/recharts/issues/466
let private renderChart (data: 'a[]) =
    Recharts.pieChart [
      pieChart.margin (15, 20, 0, 5)
      pieChart.children [
        Recharts.tooltip [ ]
        Recharts.legend [ Recharts.legend.iconType.square ]
        Recharts.pie [
          pie.data data
          pie.dataKey "count"
          pie.nameKey "description"
          pie.labelLine false
          pie.label renderLabel
          pie.fill "#8884d8"
          pie.children (cells data)
        ]
      ]
    ]

// https://fable.io/repl/#?code=LYewJgrgNgpgBAEQIYBckDUCWBnCSqYBeqmIAdgFAUgAOMZcAygJ7YozDV0MBiSARrAB0ABRAB3GACcRSAMYBrLvTh9BMIQGEQUmMt4Dh23UIBS2AJJl2U2vtWGNF4DR0p7AUSjAcAC0-efkIASjDy7rQqasKhcr5IUijY9tEasfGJ2KK2NMmRvNDASBSgkLBwwXAAvA7qQgASMFB0Ulmh4SXg0PAi1RXZtMkUAPTDcAAqvvD0YK6Y1nBzCzS2AG6YYPMA5nAoU3BgqEhwSGRgcL4ScCAAZuwMYCBwks+nKLtPNNBQcJjusO8ANoAHgAMn9pPgAHwAXTgAFVgqC+gAiXwoFA0ABcowADEJ8YSsQAOXHEgCsKIoALgIPBNmhcIQAEFxsyAPo8DzjTT1dmMDyadkWABy4w8wXQzORNQAzFRRnAALIAeQQHlBFBQzDoyvATWqFDgxrgAG9EEcsdd+AArOAAXyo2t1SuwOyqRpNAB9VDAUHFkGhPcafTw-XFQthXGRsPBbhUYLgoChgSBbQAaOAwAAeZChCrG8JELPF1L9cBu4d8gaQmmA5wAFABKQ0muA0rZ+mtwZt9FYgHyxs3Btvtv0AQjguij5CHNTD-t8Qkri4RSNpMJHbYBk5t2HIfWn0djQj35GbW5NuhQECkDDPlDbjrbdbAQluIlsg-gnZQ3Yvo4mg2KoKHAUJQr6i6RseMBNpexoNh4Ui2FIYEQQuEaJjBcFlu88x-D2LYem25rdjU-AgNmtJel6cL2pmK4Bkcr4XrhcAQDQhzsD2wBulirpbC2DalE0-H6lAREjkUq68Ts4h-P4bahlW3YALT5oBxoiVADEqcx9asUpkGYTOMbwNOcDqfBcDSXEU6Js8CnWT6IEHEclkQea2mOXsFpoH0XHHPRcCvkIZDkHomlwD6SEoTZboedZbYAEK2OIsZSEIciziAwjSHFslJSa2mZqF4VkHobG6Gc0jJQkmgZO8gWtia-D1Y1tLWQ1CQoA0MCYFs6JwLKuKEl1jVCAA6hsvkACyjQS409UI3aBSOcKAtZ2ZINmOC0poPWJpgpwrUcADSMDMHAKLZiiG2blFzA7XtgL3dZKAgLlKCYDQG6vdZciHdgx1kAA4lIGx-Q9mltahgIHYkR0nTWF1XSizAogA3HAIhCDwmBQD8KIAMTEsScgAIxk3dG7rVQNLrDA4g8eJVpKuJLabFGqB2cRJrBEImyrButJwNZjRIJsZBbEIvgUyLgIC2wqEopNTTZcA8AfbU5TrLg+BECQ5B3VQUXVWAtXtT1cDjuO2mnUGbabiMYzMjQNAUJ+IBbFISDAEIwAKF7Pt+78ZAERxXHwIz4gUF6EHB77-vyXsjAQPw2ByBDNDfQeDY3BADDAIl27lrxfQlhyXI8nyApCqK4qStKcAAFRwBTo24iOgIFw8OA0DzvgeXApiMEIsYoFYNirPgPa94Rw9cwPq4YdWRwtuX8e-Fs4W6DCcFb4nftCCnvjtHI7wok0g6+KpSDu1Sh+2CH-tSIXQA&html=DwCwLgtgNgfAUAAlAUwIYBN4O0iyyoLhgAOAtMgI4CuAlgG4C8A5AMID2AdmMt2QCoBPEsmYIAxlx7cWPAB5gA9OGgBuCSFQAnAM75G1MADMyADmZYcwHeK20SYCVvY6d7OwHNanBDq3jGACJiEh0ALkVFak4SAGsPADpJCEUtNHEwAAEARgA2KIh0VPSwBJJndGoM2i4EiG8EgCsdQJhgRRs7B0tsa1t7R1sXN09vX38gkPDI6LjE5OLUDLJ0dggc-OpCxeXViDKKqrAazjqG5tb2zoH4Pq7B51d3Wi8fPwDgsFJpqJj4pLWinK7HIYGEyB0OQArAl8sDQeCdGdThc2h1+t1EEhrg4nMNnq9xh8phFfnMASk0uJNFowJDsgkAMwJbIFIoAJWQ1O0dORTRaaJxYBgAEgscAoN5Ygg0lAgjowVAISBkPhAkQ0kZJl9QqTxOhOM0klB2NR0EYoNpkBTFKhGqg5IpJQAjHSKZ3UKAQVCKAAMCQA7CzFOJXO7Pd6+aGWghFD0JVKZcg5YEFYIlToVWqNcgtZ9vnqDUbxCazRarTa7Q6nbRXYojFIyKgAO4QtbIRQAFkDCV9IbDDb4LbbeCjrnVcbg7RVGCwwGd7HQggkltcQVQJHIkm4qG8yC0rSxvXQDAQtHQQWT9UzTc36pLqDXgSvtBvG5Il0UJ-ot3di8Ec7KJAsBAA&css=Q
// https://github.com/fable-compiler/fable-react/blob/master/src/Fable.Recharts/Fable.Recharts.fs
// https://github.com/evelinag/fableconf-data-viz
let private renderData data =
  match data with
  | Some x -> x |> renderChart
  | None -> Html.div [ prop.className "pageloader is-white is-active"; prop.style [ style.position.relative; style.minWidth (length.percent 100); style.minHeight 320 ] ]
    
// https://github.com/recharts/recharts/issues/196
let view (data: 'a[] option) =
  Recharts.responsiveContainer [
    //Recharts.responsiveContainer.width (length.percent 100)
    Recharts.responsiveContainer.aspect (4.0/3.0)
    Recharts.responsiveContainer.chart (renderData data)
  ]