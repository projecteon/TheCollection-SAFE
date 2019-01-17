module Client.Dashboard.State

open Elmish
open Fable.Core
open Fable.PowerPack
open Fable.C3
open Thoth.Json

open Client.Dashboard.Types
open Domain.Types

// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs
let getCountByBrandsCmd =
  Cmd.ofPromise
    (Fetch.fetchAs<CountBy<string> list> "/api/teabags/countby/brands" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    []
    GetCountByBrandsSuccess
    GetCountByBrandsError

let getCountBybagtypesCmd =
  Cmd.ofPromise
    (Fetch.fetchAs<CountBy<string> list> "/api/teabags/countby/bagtypes" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    []
    GetCountByBagtypesSuccess
    GetCountByBagtypesError

let mapToData (countBy: CountBy<string> list) =
  countBy
  |> List.truncate 20
  |> List.map (fun x -> [| Some (U3.Case1 x.description); Some (U3.Case3 (float x.count)) |] )
  |> Array.ofList
  |> ResizeArray
  |> Some

let toBarChart (categories: ResizeArray<string> option) (data: ResizeArray<PrimitiveArray> option) = Some {
  data = {
    columns = data
    ``type`` = Some ChartType.Bar
  }
  axis =
    match categories with
    | Some x -> Some ( { x = { ``type``= Some "category"; categories = categories }})
    | None -> None
}

let toPieChart (data: ResizeArray<PrimitiveArray> option) = Some {
  data = {
    columns = data
    ``type`` = Some ChartType.Pie
  }
  axis = None
}

let transform (chart: ChartConfig option) (tranformation: ChartTransformation) =
  match chart with
  | Some x ->
    match tranformation with
    | PieToBar -> toBarChart None x.data.columns
    | BarToPie -> toPieChart x.data.columns
  | None -> None

let init () =
    let initialModel = {
      countByBrands = None
      countByBagtypes = None
      countBrands = None
      countBagtypes = None
    }
    initialModel, getCountByBrandsCmd, getCountBybagtypesCmd

let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetCountByBrandsSuccess data ->
    { model with countByBrands = Some data; countBrands = (mapToData data |> toBarChart (Some (ResizeArray [| "brands" |]))) }, Cmd.none
  | GetCountByBrandsError exn -> model, Cmd.none
  | GetCountByBagtypesSuccess data ->
    { model with countByBagtypes = Some data; countBagtypes = (mapToData data |> toPieChart)}, Cmd.none
  | GetCountByBagtypesError exn -> model, Cmd.none
  | TransformCountByBrand transformation ->
    {model with countBrands = (transform model.countBrands transformation)}, Cmd.none
  | TransformCountByBagtype transformation ->
    {model with countBagtypes = (transform model.countBagtypes transformation)}, Cmd.none
  | ReloadBrands -> model, getCountByBrandsCmd
