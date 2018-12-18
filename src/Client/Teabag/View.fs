module Client.Teabag.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

open Client.Components
open Client.FileUrlHandler
open Client.Teabag.Types
open Services.Dtos

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

let form (teabag: Teabag) (model:Model) dispatch =
    form [AutoComplete Off] [
        (ComboBox.View.viewWithButtons model.brandCmp (BrandCmp >> dispatch))
        Field.div [ ] [
            Label.label [ Label.Option.For "flavour" ] [
                str "Flavour"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: English breakfast"; Input.DefaultValue teabag.flavour; Input.Option.Id "flavour" ]
            ]
        ]
        (ComboBox.View.viewWithoutButtons model.bagtypeCmp (BagtypeCmp >> dispatch))
        (ComboBox.View.viewWithButtons model.countryCmp (CountryCmp >> dispatch))
        Field.div [ ] [
            Label.label [ Label.Option.For "hallmark" ] [
                str "Hallmark"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: Hallmark"; Input.DefaultValue teabag.hallmark; Input.Option.Id "hallmark"; Input.OnChange (fun ev -> dispatch (HallmarkChanged ev.Value)) ]
            ]
        ]
        Field.div [ ] [
            Label.label [ Label.Option.For "serialnumber" ] [
                str "Serialnumber"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: 0123456789"; Input.DefaultValue teabag.serialnumber; Input.Option.Id "serialnumber" ]
            ]
        ]
        Field.div [ ] [
            Label.label [ Label.Option.For "serie" ] [
                str "Serie"
            ]
            Control.div [ ] [
                Input.text [ Input.Placeholder "Ex: With logo"; Input.DefaultValue teabag.serie; Input.Option.Id "serie" ]
            ]
        ]
    ]

let view (model:Model) (dispatch: Msg -> unit) =
    [
        yield div [ Class "columns is-mobile is-multiline" ] [
            match model.data with
            | Some x ->
                yield div [ Class "column is-half-desktop is-full-mobile" ] [
                    yield Content.content [] []
                    yield img [ Src (getUrl x.imageid) ]
                ]
                yield div [ Class "column is-half-desktop is-full-mobile" ] [
                    yield Content.content [] []
                    yield Card.card [] [ Card.content [] [ form x model dispatch ] ]
                ]
            | _ ->
                yield div [ Class "column is-centered" ] [
                    yield Content.content [] []
                    yield div [ classList [ "is-active", 1 = 1 ] ] [ str "unloaded" ]
                ]
        ]
    ]
