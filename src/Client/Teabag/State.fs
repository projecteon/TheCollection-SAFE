module Client.Teabag.State

open Elmish
open Fable.PowerPack
open Thoth.Json

open Client.Components
open Client.Teabag.Types
open Services.Dtos


// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs

let getTeabagCmd id =
  Cmd.ofPromise
    (Fetch.fetchAs<Teabag> (sprintf "/api/teabags/%s" id) (Decode.Auto.generateDecoder<Teabag>()) )
    []
    GetSuccess
    GetError

let init () =
    let brandCmp = ComboBox.State.init("Brand", "/api/brands")
    let bagtypeCmp = ComboBox.State.init("Bagtype", "/api/bagtypes")
    let countryCmp = ComboBox.State.init("Country", "/api/country")
    let initialModel = {
      data = None
      brandCmp = brandCmp
      bagtypeCmp = bagtypeCmp
      countryCmp = countryCmp
    }
    initialModel, getTeabagCmd

let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetSuccess data ->
    { model with data = Some data }, Cmd.batch [  Cmd.map BrandCmp (ComboBox.State.setValueCmd data.brand)
                                                  Cmd.map BagtypeCmp (ComboBox.State.setValueCmd data.bagtype)
                                                  Cmd.map CountryCmp (ComboBox.State.setValueCmd data.country)
                                              ]
  | HallmarkChanged hallmark ->
    match model.data with
    | Some x -> { model with data = Some { x with hallmark = hallmark } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with hallmark = hallmark } }, Cmd.none
  | BrandCmp msg ->
    let res, cmd = ComboBox.State.update msg model.brandCmp
    let nextTeabag =
      match model.data with
      | Some x -> Some {x with brand = res.Value}
      | _ -> None

    let nextModel = { model with brandCmp = res; data = nextTeabag }
    nextModel, Cmd.map BrandCmp cmd
  | BagtypeCmp msg ->
    let res, cmd = ComboBox.State.update msg model.bagtypeCmp
    let nextTeabag =
      match model.data with
      | Some x -> Some {x with bagtype = res.Value}
      | _ -> None

    let nextModel = { model with bagtypeCmp = res; data = nextTeabag }
    nextModel, Cmd.map BagtypeCmp cmd
  | CountryCmp msg ->
    let res, cmd = ComboBox.State.update msg model.countryCmp
    let nextTeabag =
      match model.data with
      | Some x -> Some {x with country = res.Value}
      | _ -> None

    let nextModel = { model with countryCmp = res; data = nextTeabag }
    nextModel, Cmd.map CountryCmp cmd
  | _ -> model, Cmd.none
