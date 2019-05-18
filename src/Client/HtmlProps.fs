module HtmlProps

open Fable.Helpers.React.Props

type AutoComplete =
 | Off
 | On
 override this.ToString () =
        match this with
        | Off -> "off"
        | On -> "on"

type HTMLAttr =
     | [<CompiledName("autocomplete")>] AutoComplete of AutoComplete
     interface IHTMLProp

