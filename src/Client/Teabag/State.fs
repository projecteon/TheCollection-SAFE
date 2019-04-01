module Client.Teabag.State

open Elmish
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Client.Components
open Client.Teabag.Types
open Client.Util
open Services
open Services.Dtos
open Domain.SharedTypes

let mapValidationErrors validationError =
  match validationError with
  | ValidationErrors.BrandError x -> x
  | ValidationErrors.BagtypeError x -> x
  | ValidationErrors.FlavourError x -> x

let isValid (validationErrors: ValidationErrors seq) =
  validationErrors |> Seq.isEmpty

let private validateAndMapResult (doValidation: bool, errorType: (string -> ValidationErrors), value: 'a, validateFunc: ('a -> Result<'a, string>)) =
  if doValidation then
    match (validateFunc value) with
    | Success x -> None
    | Failure y -> y |> errorType |> Some
  else
    None

let private validate (doValidation: bool, teabag: Teabag) =
  let validationErrors = seq [
    validateAndMapResult (doValidation, ValidationErrors.BrandError, teabag.brand, RefValueValidation.validate)
    validateAndMapResult (doValidation, ValidationErrors.BagtypeError, teabag.bagtype, RefValueValidation.validate)
  ]
  validationErrors |> Seq.choose id

// https://stackoverflow.com/questions/3363184/f-how-to-elegantly-select-and-group-discriminated-unions
let fetchAndMapError (model: Model) (matcher: (ValidationErrors -> bool)) =
  model.validationErrors
  |> Seq.filter matcher
  |> Seq.map mapValidationErrors

let fetchBrandErrors (model: Model) =
  fetchAndMapError model (fun x -> match x with | BrandError y -> true | _ -> false)

let fetchBagtypeErrors (model: Model) =
  fetchAndMapError model (fun x -> match x with | BagtypeError y -> true | _ -> false)

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

let uploadImage (file : Fable.Import.Browser.File) (token: JWT) =
    let formData = Fable.Import.Browser.FormData.Create()
    formData.append(file.name, file)
    let defaultProps =
        [ RequestProperties.Method HttpMethod.POST
          RequestProperties.Body (formData |> unbox)
          Fetch.requestHeaders [
              HttpRequestHeaders.Authorization ("Bearer " + token.String)
              HttpRequestHeaders.ContentType "multipart/form-data; charset=utf-8"
          ]]
    let decoder = (Decode.Auto.generateDecoder<string>())
    Cmd.ofPromise (Fetch.fetchAs "/api/teabags/upload" decoder) defaultProps
      UploadSuccess
      UploadError

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
      fetchError = None
      doValidation = false
      validationErrors = Seq.empty
    }
    initialModel, getTeabagCmd

let valueOrDefault (value: RefValue option): RefValue =
  match value with
  | Some x -> x
  | None -> EmptyRefValue

let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetSuccess data ->
    { model with data = Some data; doValidation = true }, Cmd.batch [ Cmd.map BrandCmp (ComboBox.State.setValueCmd (data.brand |> Some))
                                                                      Cmd.map BagtypeCmp (ComboBox.State.setValueCmd (data.bagtype |> Some))
                                                                      Cmd.map CountryCmp (ComboBox.State.setValueCmd data.country) ]
  | GetError exn -> { model with fetchError = Some exn }, Cmd.none
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
    let res, cmd, exMsg = ComboBox.State.update msg model.bagtypeCmp
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
  | ImageChanged input ->
    model, tryAuthorizationRequest (input |> uploadImage) model.userData
  | UploadSuccess returnMsg -> model, Cmd.none
  | UploadError exn -> { model with fetchError = Some exn }, Cmd.none
  | Validate ->
    let validatedModel =
      match model.data with
        | Some x -> { model with validationErrors = validate (model.doValidation, x)}
        | None -> model
    validatedModel, Cmd.batch [ Cmd.map BrandCmp (validatedModel |> fetchBrandErrors |> ComboBox.State.setErrors)
                                Cmd.map BagtypeCmp (validatedModel |> fetchBagtypeErrors |> ComboBox.State.setErrors)  ]
  | ValidateAndSave ->
    let newModel = { model with doValidation = true }
    let validatedModel =
      match model.data with
        | Some x -> { model with validationErrors = validate (newModel.doValidation, x)}
        | None -> newModel
    validatedModel, Cmd.batch [ Cmd.map BrandCmp (validatedModel |> fetchBrandErrors |> ComboBox.State.setErrors)
                                Cmd.map BagtypeCmp (validatedModel |> fetchBagtypeErrors |> ComboBox.State.setErrors) ]
