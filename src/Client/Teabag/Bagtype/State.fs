module Client.Teabag.Bagtype.State

open Elmish
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

open Domain.SharedTypes
open Client.Teabag.Bagtype.Types
open Client.Util
open Services
open Services.Dtos

let areEqual original current =
  match original, current with
  | Some o, Some c -> o = c
  | None, Some c -> false
  | Some o, None -> false
  | None, None -> true

let validationRules (bagtype: Bagtype) =
  seq [
     (ValidationErrors.NameError, bagtype.name, NameValidation.validate)
  ]

let private validate (bagtype: Bagtype) =
  bagtype
  |> validationRules
  |> Seq.map validateAndMapResult
  |> Seq.choose id

let isValid (validationErrors: ValidationErrors seq) =
  validationErrors |> Seq.isEmpty

let private getBagtypeCmd (id: DbId option) (token: JWT) =
  match id with
  | Some id when id.Int > 0 ->
    Cmd.ofPromise
      (Fetch.fetchAs<Bagtype> (sprintf "/api/bagtypes/%i" id.Int) (Decode.Auto.generateDecoder<Bagtype>()) )
      [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetSuccess
      GetError
  | _ -> Cmd.ofMsg (GetSuccess NewBagtype)

let private saveBagtypeCmd (bagtype: Bagtype) (token: JWT) =
  let (requestMethod, url) =
    match bagtype.id with
    | DbId 0 -> (HttpMethod.POST, "/api/bagtypes")
    | _ -> (HttpMethod.PUT, sprintf "/api/bagtypes/%i" bagtype.id.Int)
  let fetchProperties =
    [ RequestProperties.Method requestMethod
      RequestProperties.Body !^(Encode.Auto.toString(0, bagtype))
      Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
      ]]
  let decoder = (Decode.Auto.generateDecoder<int>())
  Cmd.ofPromise (Fetch.fetchAs url decoder) fetchProperties
    SaveSuccess
    SaveFailure


let private trySave (model: Model) =
  match (isValid model.validationErrors), model.data with
  | true, Some bagtype -> Cmd.ofMsg (Msg.Save bagtype)
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
  initialModel, getBagtypeCmd

let update (msg:Msg) model : Model*Cmd<Msg>*ExternalMsg =
  match msg with
  | GetSuccess data ->
    let newModel = { model with originaldata = Some data; data = Some data; doValidation = false; isWorking = false }
    newModel, Cmd.none, ExternalMsg.UnChanged
  | GetError exn -> { model with fetchError = Some exn; isWorking = false }, Cmd.none, ExternalMsg.UnChanged
  | NameChanged newName ->
    match model.data with
    | Some x -> { model with data = Some { x with name = Name newName } }, Cmd.ofMsg Validate, ExternalMsg.UnChanged
    | _ -> { model with data = Some { NewBagtype with name = Name newName } }, Cmd.none, ExternalMsg.UnChanged
  | Save bagtype -> model, tryAuthorizationRequest (saveBagtypeCmd bagtype) model.userData, ExternalMsg.UnChanged
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
