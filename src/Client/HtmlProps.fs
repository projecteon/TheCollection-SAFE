module HtmlProps

open Fable.React.Props

type AutoComplete =
 | Off
 | On
 override this.ToString () =
        match this with
        | Off -> "off"
        | On -> "on"

type HTMLAttr =
     | [<CompiledName("autoComplete")>] AutoComplete of AutoComplete
     interface IHTMLProp

