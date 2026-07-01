module HtmlProps

open Fable.React.Props

type AutoCompleteValue =
 | Off
 | On
 override this.ToString () =
        match this with
        | Off -> "off"
        | On -> "on"

// Fable.React 9 removed the IHTMLProp interface; custom attributes are now
// expressed via HTMLAttr.Custom.
let AutoComplete (value: AutoCompleteValue) : HTMLAttr =
    HTMLAttr.Custom ("autoComplete", string value)
