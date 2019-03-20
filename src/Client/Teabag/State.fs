module Client.Teabag.State

open Elmish
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Client.Components
open Client.Teabag.Types
open Services.Dtos
open Domain.SharedTypes


// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs

let getTeabagCmd (id: int option) (token: JWT) =
  match id with
  | Some id ->
    Cmd.ofPromise
      (Fetch.fetchAs<Teabag> (sprintf "/api/teabags/%i" id) (Decode.Auto.generateDecoder<Teabag>()) )
      [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetSuccess
      GetError
  | None -> Cmd.ofMsg (GetSuccess NewTeabag)

let init (userData: UserData option) =
    let brandCmp = ComboBox.State.init("Brand", RefValueTypes.Brand, userData)
    let bagtypeCmp = ComboBox.State.init("Bagtype", RefValueTypes.Bagtype, userData)
    let countryCmp = ComboBox.State.init("Country", RefValueTypes.Country, userData)
    let initialModel = {
      data = None
      brandCmp = brandCmp
      bagtypeCmp = bagtypeCmp
      countryCmp = countryCmp
      userData = userData
    }
    initialModel, getTeabagCmd

let valueOrDefault (value: RefValue option): RefValue =
  match value with
  | Some x -> x
  | None -> EmptyRefValue

let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetSuccess data ->
    { model with data = Some data }, Cmd.batch [  Cmd.map BrandCmp (ComboBox.State.setValueCmd (data.brand |> Some))
                                                  Cmd.map BagtypeCmp (ComboBox.State.setValueCmd (data.bagtype |> Some))
                                                  Cmd.map CountryCmp (ComboBox.State.setValueCmd data.country)
                                              ]
  | FlavourChanged flavour ->
    match model.data with
    | Some x -> { model with data = Some { x with flavour = flavour } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with flavour = flavour } }, Cmd.none
  | HallmarkChanged hallmark ->
    match model.data with
    | Some x -> { model with data = Some { x with hallmark = hallmark } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with hallmark = hallmark } }, Cmd.none
  | SerialnumberChanged serialnumber ->
    match model.data with
    | Some x -> { model with data = Some { x with serialnumber = serialnumber } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with serialnumber = serialnumber } }, Cmd.none
  | SerieChanged serie ->
    match model.data with
    | Some x -> { model with data = Some { x with serie = serie } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with serie = serie } }, Cmd.none
  | BrandCmp msg ->
    let res, cmd, exMsg = ComboBox.State.update msg model.brandCmp
    let nextTeabag =
      match exMsg with
      | ComboBox.Types.ExternalMsg.UnChanged-> model.data
      | ComboBox.Types.ExternalMsg.OnChange newValue ->
        model.data
        |> function
          | Some x ->  Some {x with brand = newValue |> valueOrDefault}
          | _ -> model.data

    let nextModel = { model with brandCmp = res; data = nextTeabag }
    nextModel, Cmd.map BrandCmp cmd
  | BagtypeCmp msg ->
    let res, cmd, exMsg = ComboBox.State.update msg model.bagtypeCmp
    let nextTeabag =
      match exMsg with
      | ComboBox.Types.ExternalMsg.UnChanged-> model.data
      | ComboBox.Types.ExternalMsg.OnChange newValue ->
        model.data
        |> function
          | Some x -> Some {x with bagtype = newValue |> valueOrDefault}
          | _ -> model.data

    let nextModel = { model with bagtypeCmp = res; data = nextTeabag }
    nextModel, Cmd.map BagtypeCmp cmd
  | CountryCmp msg ->
    let res, cmd, exMsg = ComboBox.State.update msg model.countryCmp
    let nextTeabag =
      match exMsg with
      | ComboBox.Types.ExternalMsg.UnChanged-> model.data
      | ComboBox.Types.ExternalMsg.OnChange newValue ->
        model.data
        |> function
          | Some x -> Some {x with country = newValue}
          | _ -> model.data
    
    let nextModel = { model with countryCmp = res; data = nextTeabag }
    nextModel, Cmd.map CountryCmp cmd
  | _ -> model, Cmd.none
