namespace Client.Recharts

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React

[<Erase>]
type IWrapperStyleAttribute = interface end

type DataCount = Ten=10 | Twenty=20

[<RequireQualifiedAccess>]
module Interop =
    let inline mkLegendAttr (key: string) (value: obj) : Feliz.Recharts.ILegendProperty = unbox (key, value)
    let inline mkWrapperStyleAttr (key: string) (value: obj) : IWrapperStyleAttribute = unbox (key, value)

module Extensions =
  type LegenDataPayload = {
    stroke: string
    name: string
    value: int
  }

  type LegendData = {
    active: bool
    payload: LegenDataPayload array
  }
      
  type Feliz.Recharts.tooltip with
      static member inline content (render: LegendData -> ReactElement) = Feliz.Recharts.Interop.mkTooltipAttr "content" render

  type Feliz.Recharts.xAxis with
      static member inline angle (value: float) = Feliz.Recharts.Interop.mkXAxisAttr "angle" value

  type Feliz.Recharts.line with
    static member inline key (value: string) = Feliz.Recharts.Interop.mkLineAttr "key" value
    static member inline strokeOpacity (value: float) = Feliz.Recharts.Interop.mkLineAttr "strokeOpacity" value

  type wrapperStyle =
    static member inline bottom(value: int) = Interop.mkWrapperStyleAttr "bottom" ((unbox<string> value) + "px")
    static member inline top(value: int) = Interop.mkWrapperStyleAttr "top" ((unbox<string> value) + "px")
    static member inline right(value: int) = Interop.mkWrapperStyleAttr "right" ((unbox<string> value) + "px")
    static member inline left(value: int) = Interop.mkWrapperStyleAttr "left" ((unbox<string> value) + "px")

  type legend =
    static member inline wrapperStyle (properties: IWrapperStyleAttribute list) = Interop.mkLegendAttr "wrapperStyle" (createObj !!properties)
    static member inline onMouseEnter (handler: MouseEvent -> unit) = Interop.mkLegendAttr "onMouseEnter" handler
    static member inline onMouseLeave (handler: MouseEvent -> unit) = Interop.mkLegendAttr "onMouseLeave" handler


module ColorHelpers =
  let LineColors = [|"#0088FE"; "#00C49F"; "#FFBB28"; "#FF8042"; "#8884d8"|];
  let C3ColorsExtended = [|"#1f77b4"; "#aec7e8"; "#ff7f0e"; "#ffbb78"; "#2ca02c"; "#98df8a"; "#d62728"; "#ff9896"; "#9467bd"; "#c5b0d5"; "#8c564b"; "#c49c94"; "#e377c2"; "#f7b6d2"; "#7f7f7f"; "#c7c7c7"; "#bcbd22"; "#dbdb8d"; "#17becf"; "#9edae5" |];
  let C3Colors = [|"#1f77b4"; "#ff7f0e"; "#2ca02c"; "#d62728"; "#9467bd"; "#8c564b"; "#e377c2"; "#7f7f7f"; "#bcbd22"; "#17becf" |];

  let getColor (colors: string array) x =
    let colorCount = C3Colors |> Array.length
    let rec tailRecursiveFactorial acc =
      if acc < colorCount then
        colors.[acc]
      else
        tailRecursiveFactorial (acc - colorCount)
    tailRecursiveFactorial x
