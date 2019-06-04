module Client.Teabag.State

open Elmish
open Fable.Core.JsInterop
open Fetch
//open Fable.PowerPack.Keyboard
open Thoth.Json

open Client.Components
open Client.Teabag.Types
open Client.ElmishHelpers
open Client.Validation
open Server.Api
open Server.Api.Dtos
open Domain.SharedTypes

let private valueOrDefault (value: RefValue option): RefValue =
  match value with
  | Some x -> x
  | None -> EmptyRefValue

let private refValueToOptional (value: RefValue): RefValue option =
  if value = EmptyRefValue then None
  else Some value

let private mapValidationErrors validationError =
  match validationError with
  | ValidationErrors.BrandError x -> x
  | ValidationErrors.BagtypeError x -> x
  | ValidationErrors.FlavourError x -> x

let private validate (teabag: Teabag) =
  let validationErrors = seq [
    validateAndMapResult (ValidationErrors.BrandError, teabag.brand, RefValueValidation.validate)
    validateAndMapResult (ValidationErrors.BagtypeError, teabag.bagtype, RefValueValidation.validate)
  ]
  validationErrors |> Seq.choose id

// https://stackoverflow.com/questions/3363184/f-how-to-elegantly-select-and-group-discriminated-unions
let private fetchAndMapError (model: Model) (matcher: (ValidationErrors -> bool)) =
  model.validationErrors
  |> Seq.filter matcher
  |> Seq.map mapValidationErrors

let private fetchBrandErrors (model: Model) =
  fetchAndMapError model (fun x -> match x with | BrandError y -> true | _ -> false)

let private fetchBagtypeErrors (model: Model) =
  fetchAndMapError model (fun x -> match x with | BagtypeError y -> true | _ -> false)

let isValid (validationErrors: ValidationErrors seq) =
  validationErrors |> Seq.isEmpty

// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs
let private getTeabagCmd (id: int option) (token: JWT) =
  match id with
  | Some id when id > 0 ->
    Cmd.OfPromise.either
      (Client.Http.fetchAs<Teabag> (sprintf "/api/teabags/%i" id) (Decode.Auto.generateDecoder<Teabag>()) )
      [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetSuccess
      GetError
  | _ -> Cmd.ofMsg (GetSuccess NewTeabag)

let private reloadTeabagCmd (teabag: Teabag option) (token: JWT) =
  match teabag with
  | None -> getTeabagCmd None token
  | Some x -> getTeabagCmd (Some x.id.Int) token

// https://github.com/fable-compiler/fable-powerpack/blob/master/src/Fetch.fs
// https://stackoverflow.com/questions/36067767/how-do-i-upload-a-file-with-the-js-fetch-api
let private uploadImage (file : Browser.Types.File) (token: JWT) =
  let formData = Browser.XMLHttpRequest.FormData.Create()
  formData.append(file.name, file)
  let defaultProps =
    [ RequestProperties.Method HttpMethod.POST
      RequestProperties.Body (formData |> unbox)
      Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
      ]]
  let decoder = (Decode.Auto.generateDecoder<ImageId>())
  Cmd.OfPromise.either (Client.Http.fetchAs "/api/teabags/upload" decoder) defaultProps
    ImageChanged
    UploadError

let private saveTeabagCmd (teabag: Teabag) (token: JWT) =
  let (requestMethod, url) =
    match teabag.id with
    | DbId 0 -> (HttpMethod.POST, "/api/teabags")
    | _ -> (HttpMethod.PUT, sprintf "/api/teabags/%i" teabag.id.Int)
  let fetchProperties =
    [ RequestProperties.Method requestMethod
      RequestProperties.Body !^(Encode.Auto.toString(0, teabag))
      Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        //HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
  let decoder = (Decode.Auto.generateDecoder<int>())
  Cmd.OfPromise.either (Client.Http.fetchAs url decoder) fetchProperties
    SaveSuccess
    SaveFailure


let private trySave (model: Model) =
  match (isValid model.validationErrors), model.data with
  | true, Some teabag -> Cmd.ofMsg (Msg.Save teabag)
  | _, _-> Cmd.none

let init (userData: UserData option) =
  let brandCmp = ComboBox.State.init("Brand", RefValueTypes.Brand)
  let bagtypeCmp = ComboBox.State.init("Bagtype", RefValueTypes.Bagtype)
  let countryCmp = ComboBox.State.init("Country", RefValueTypes.Country)
  let initialModel = {
    originaldata = None
    data = None
    brandCmp = brandCmp
    bagtypeCmp = bagtypeCmp
    countryCmp = countryCmp
    fetchError = None
    doValidation = false
    validationErrors = Seq.empty
    isWorking = true
    editBagtypeCmp = None
    editBrandCmp = None
    editCountryCmp = None
  }
  initialModel, getTeabagCmd

let update (msg:Msg) model userData : Model*Cmd<Msg> =
  match msg with
  | GetSuccess data ->
    let newModel = { model with originaldata = Some data; data = Some data; doValidation = false; isWorking = false }
    newModel, Cmd.batch [ Cmd.map BrandCmp (ComboBox.State.setValueCmd (data.brand |> refValueToOptional))
                          Cmd.map BagtypeCmp (ComboBox.State.setValueCmd (data.bagtype |> refValueToOptional))
                          Cmd.map CountryCmp (ComboBox.State.setValueCmd data.country) ]
  | GetError exn -> { model with fetchError = Some exn; isWorking = false }, Cmd.none
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
    let res, cmd, exMsg = ComboBox.State.update msg model.brandCmp userData
    let nextTeabag, nextCmd =
      match exMsg with
      | ComboBox.Types.ExternalMsg.UnChanged-> model.data, Cmd.map BrandCmp cmd
      | ComboBox.Types.ExternalMsg.OnChange newValue ->
        model.data
        |> function
          | Some x ->  Some {x with brand = newValue |> valueOrDefault}, Cmd.batch [Cmd.map BrandCmp cmd; Cmd.ofMsg Validate]
          | _ -> model.data, Cmd.map BrandCmp cmd

    let nextModel = { model with brandCmp = res; data = nextTeabag }
    nextModel, nextCmd
  | BagtypeCmp msg ->
    let res, cmd, exMsg = ComboBox.State.update msg model.bagtypeCmp userData
    let nextTeabag, nextCmd =
      match exMsg with
      | ComboBox.Types.ExternalMsg.UnChanged-> model.data, Cmd.map BagtypeCmp cmd
      | ComboBox.Types.ExternalMsg.OnChange newValue ->
        model.data
        |> function
          | Some x -> Some {x with bagtype = newValue |> valueOrDefault}, Cmd.batch [Cmd.map BagtypeCmp cmd; Cmd.ofMsg Validate]
          | _ -> model.data, Cmd.map BagtypeCmp cmd

    let nextModel = { model with bagtypeCmp = res; data = nextTeabag }
    nextModel, nextCmd
  | CountryCmp msg ->
    let res, cmd, exMsg = ComboBox.State.update msg model.countryCmp userData
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
  | ImageChanged id ->
    match model.data with
    | Some x -> { model with data = Some { x with imageid = id } }, Cmd.none
    | _ -> { model with data = Some { NewTeabag with imageid = id } }, Cmd.none
  | Upload input ->
    model, tryJwtCmd (input |> uploadImage) userData
  | UploadError exn -> { model with fetchError = Some exn }, Cmd.none
  | Save teabag -> model, tryJwtCmd (saveTeabagCmd teabag) userData
  | SaveSuccess id ->
    let updatedTeabag = Some { model.data.Value with id = DbId id }
    { model with data = updatedTeabag; originaldata = updatedTeabag }, Cmd.none
  | SaveFailure exn -> { model with fetchError = Some exn }, Cmd.none
  | Validate ->
    let validatedModel =
      match model.data, model.doValidation with
        | Some x, true -> { model with validationErrors = validate x}
        | _, _ -> model
    validatedModel, Cmd.batch [ Cmd.map BrandCmp (validatedModel |> fetchBrandErrors |> ComboBox.State.setErrors)
                                Cmd.map BagtypeCmp (validatedModel |> fetchBagtypeErrors |> ComboBox.State.setErrors)  ]
  | ValidateAndSave ->
    let newModel = { model with doValidation = true }
    let validatedModel =
      match model.data, newModel.doValidation with
        | Some x, true -> { newModel with validationErrors = validate x}
        | _ -> newModel
    validatedModel, Cmd.batch [ Cmd.map BrandCmp (validatedModel |> fetchBrandErrors |> ComboBox.State.setErrors)
                                Cmd.map BagtypeCmp (validatedModel |> fetchBagtypeErrors |> ComboBox.State.setErrors)
                                trySave validatedModel]
  | Reload -> model, tryJwtCmd (reloadTeabagCmd model.data) userData
  | ToggleAddBagtypeModal display ->
    if display then
      let bagtypeModel, cmd = Client.Teabag.Bagtype.State.init userData
      let bagtypeId = match model.data with | Some x -> Some x.bagtype.id | None -> None
      {model with editBagtypeCmp = Some bagtypeModel}, Cmd.map EditBagtypeCmp (cmd bagtypeId userData.Value.Token)
    else
      {model with editBagtypeCmp = None}, Cmd.none
  | ToggleAddBrandModal display ->
    if display then
      let brandModel, cmd = Client.Teabag.Brand.State.init userData
      let brandId = match model.data with | Some x -> Some x.brand.id | None -> None
      {model with editBrandCmp = Some brandModel}, Cmd.map EditBrandCmp (cmd brandId userData.Value.Token)
    else
      {model with editBrandCmp = None}, Cmd.none
  | ToggleAddCountryModal display ->
    if display then
      let countryModel, cmd = Client.Teabag.Country.State.init userData
      let countryId = match model.data with | Some x when x.country.IsSome -> Some x.country.Value.id | _ -> None
      {model with editCountryCmp = Some countryModel}, Cmd.map EditCountryCmp (cmd countryId userData.Value.Token)
    else
      {model with editCountryCmp = None}, Cmd.none
  | EditBagtypeCmp msg ->
    let res, cmd, exMsg = Client.Teabag.Bagtype.State.update msg model.editBagtypeCmp.Value
    let newModel, nextCmd = match exMsg, model.data with
                            | Client.Teabag.Bagtype.Types.ExternalMsg.OnChange newBagtype, Some teabag -> { model with data = Some {teabag with bagtype = newBagtype };  editBagtypeCmp = None }, Cmd.map BagtypeCmp (ComboBox.State.setValueCmd (newBagtype |> refValueToOptional))
                            | _ -> {model with editBagtypeCmp = Some res}, Cmd.none
    newModel, Cmd.batch [ nextCmd
                          Cmd.map EditBagtypeCmp cmd ]
  | EditBrandCmp msg ->
    let res, cmd, exMsg = Client.Teabag.Brand.State.update msg model.editBrandCmp.Value
    let newModel, nextCmd = match exMsg, model.data with
                            | Client.Teabag.Brand.Types.ExternalMsg.OnChange newBrand, Some teabag -> { model with data = Some {teabag with brand = newBrand };  editBrandCmp = None }, Cmd.map BrandCmp (ComboBox.State.setValueCmd (newBrand |> refValueToOptional))
                            | _ -> {model with editBrandCmp = Some res}, Cmd.none
    newModel, Cmd.batch [ nextCmd
                          Cmd.map EditBrandCmp cmd ]
  | EditCountryCmp msg ->
    let res, cmd, exMsg = Client.Teabag.Country.State.update msg model.editCountryCmp.Value
    let newModel, nextCmd = match exMsg, model.data with
                            | Client.Teabag.Country.Types.ExternalMsg.OnChange newCountry, Some teabag -> { model with data = Some {teabag with brand = newCountry };  editCountryCmp = None }, Cmd.map CountryCmp (ComboBox.State.setValueCmd (newCountry |> refValueToOptional))
                            | _ -> {model with editCountryCmp = Some res}, Cmd.none
    newModel, Cmd.batch [ nextCmd
                          Cmd.map EditCountryCmp cmd ]

