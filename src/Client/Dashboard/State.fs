module Client.Dashboard.State

open Elmish
open Fable.PowerPack
open Thoth.Json

open Client.Dashboard.Types
open Domain.Types

// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs

let getCountByBrandsCmd =
  Cmd.ofPromise
    (Fetch.fetchAs<CountBy<string> list> (sprintf "/api/teabags/countby/brands") (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    []
    GetCountByBrandsSuccess
    GetCountByBrandsError

let getCountBybagtypesCmd =
  Cmd.ofPromise
    (Fetch.fetchAs<CountBy<string> list> (sprintf "/api/teabags/countby/bagtypes") (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    []
    GetCountByBagtypesSuccess
    GetCountByBagtypesError

let init () =
    let initialModel = {
      countByBrands = None
      countByBagtypes = None
    }
    initialModel, getCountByBrandsCmd, getCountBybagtypesCmd

let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetCountByBrandsSuccess data ->
    { model with countByBrands = Some data }, Cmd.none
  | GetCountByBrandsError exn -> model, Cmd.none
  | GetCountByBagtypesSuccess data ->
    { model with countByBagtypes = Some data }, Cmd.none
  | GetCountByBagtypesError exn -> model, Cmd.none
