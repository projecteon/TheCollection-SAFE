module Client.Teabag.Brand.State

open Elmish
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Domain.SharedTypes
open Client.ElmishHelpers
open Client.Validation
open Client.Teabag.Brand.Types
open Server.Api
open Server.Api.Dtos

let areEqual original current =
  match original, current with
  | Some o, Some c -> o = c
  | None, Some c -> false
  | Some o, None -> false
  | None, None -> true

let validationRules (brand: Brand) =
  seq [
     (ValidationErrors.NameError, brand.name, NameValidation.validate)
  ]

let private validate (brand: Brand) =
  brand
  |> validationRules
  |> Seq.map validateAndMapResult
  |> Seq.choose id

let isValid (validationErrors: ValidationErrors seq) =
  validationErrors |> Seq.isEmpty
  
let getNameErrors validationErrors =
  validationErrors
  |> Seq.choose
    (function
    | ValidationErrors.NameError err -> Some err
    | _ -> None)

let private getBrandCmd (id: DbId option) (token: JWT) =
  match id with
  | Some id when id.Int > 0 ->
    Cmd.ofPromise
      (Fetch.fetchAs<Brand> (sprintf "/api/brands/%i" id.Int) (Decode.Auto.generateDecoder<Brand>()) )
      [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetSuccess
      GetError
  | _ -> Cmd.ofMsg (GetSuccess NewBrand)

let private saveBrandCmd (brand: Brand) (token: JWT) =
  let (requestMethod, url) =
    match brand.id with
    | DbId 0 -> (HttpMethod.POST, "/api/brands")
    | _ -> (HttpMethod.PUT, sprintf "/api/brands/%i" brand.id.Int)
  let fetchProperties =
    [ RequestProperties.Method requestMethod
      RequestProperties.Body !^(Encode.Auto.toString(0, brand))
      Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
      ]]
  let decoder = (Decode.Auto.generateDecoder<int>())
  Cmd.ofPromise (Fetch.fetchAs url decoder) fetchProperties
    SaveSuccess
    SaveFailure


let private trySave (model: Model) =
  match (isValid model.validationErrors), model.data with
  | true, Some brand -> Cmd.ofMsg (Msg.Save brand)
  | _, _-> Cmd.none


let init (userData: UserData option) =
  let initialModel = {
    originaldata = None
    data = None
    userData = userData
    fetchError = None
    doValidation = false
    validationErrors = Seq.empty
    isWorking = true
  }
  initialModel, getBrandCmd

let update (msg:Msg) model : Model*Cmd<Msg>*ExternalMsg =
  match msg with
  | New -> {model with data = Some NewBrand}, Cmd.none, ExternalMsg.UnChanged
  | GetSuccess data ->
    let newModel = { model with originaldata = Some data; data = Some data; doValidation = false; isWorking = false }
    newModel, Cmd.none, ExternalMsg.UnChanged
  | GetError exn -> { model with fetchError = Some exn; isWorking = false }, Cmd.none, ExternalMsg.UnChanged
  | NameChanged newName ->
    match model.data with
    | Some x -> { model with data = Some { x with name = Name newName } }, Cmd.ofMsg Validate, ExternalMsg.UnChanged
    | _ -> { model with data = Some { NewBrand with name = Name newName } }, Cmd.none, ExternalMsg.UnChanged
  | Save brand -> model, tryJwtCmd (saveBrandCmd brand) model.userData, ExternalMsg.UnChanged
  | SaveSuccess id ->
    let updatedTeabag = Some { model.data.Value with id = DbId id }
    { model with data = updatedTeabag; originaldata = updatedTeabag }, Cmd.none, ExternalMsg.OnChange { id = updatedTeabag.Value.id; description = updatedTeabag.Value.name.String }
  | SaveFailure exn -> { model with fetchError = Some exn }, Cmd.none, ExternalMsg.UnChanged
  | Validate ->
    let validatedModel =
      match model.data, model.doValidation with
        | Some x, true -> { model with validationErrors = validate x}
        | _, _ -> model
    validatedModel, Cmd.none, ExternalMsg.UnChanged
  | ValidateAndSave ->
    let newModel = { model with doValidation = true }
    let validatedModel =
      match model.data, newModel.doValidation with
        | Some x, true -> { newModel with validationErrors = validate x}
        | _ -> newModel
    validatedModel, trySave validatedModel, ExternalMsg.UnChanged
