module Client.Dashboard.State

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Fable.C3
open Fable.Import.Moment.Moment
open Thoth.Json

open Client.Extensions
open Client.Util
open Client.Dashboard.Types
open Domain.SharedTypes
open Services.Dtos

let fetchYear (yearData: CountBy<Moment> list) =
  (Seq.head yearData).description.format("YYYY")
  |> U3.Case1
  |> Some

let mapCounts (yearData: CountBy<Moment> list): PrimitiveArray =
  yearData
  |> Seq.map (fun x -> x.count |> float |> U3.Case3 |> Some)
  |> Array.ofSeq

let createChartData (yearData: CountBy<Moment> list): PrimitiveArray =
  [| (fetchYear yearData) |]
  |> Array.append <| (mapCounts yearData)

let splitPeriodData (model: CountBy<Moment> list) =
  model
  |> Seq.ofList
  |> (Seq.split 12)

let splitData (model: CountBy<Moment> list) =
  model
  |> splitPeriodData
  |> List.map (fun x -> x |> createChartData)
  |> Array.ofList
    
let createXAxis (model: CountBy<Moment> list) =
  model
  |> Seq.ofList
  |> Seq.take 12
  |> Seq.map (fun x -> x.description.clone().year(1900.0).format("YYYY-MM-DD"))

// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs
let getCountByBrandsCmd (token: JWT) =
  Cmd.ofPromise
    (Fetch.fetchAs<CountBy<string> list> "/api/teabags/countby/brands" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
    ]]
    GetCountByBrandsSuccess
    GetCountByBrandsError

let getCountByBagtypesCmd (userData: UserData option) =
  match userData with
  | Some x ->
    Cmd.ofPromise
      (Fetch.fetchAs<CountBy<string> list> "/api/teabags/countby/bagtypes" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
      [Fetch.requestHeaders [
          HttpRequestHeaders.Authorization ("Bearer " + x.Token.String)
          HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetCountByBagtypesSuccess
      GetCountByBagtypesError
    | _ -> Cmd.none

let getCountByInsertedCmd (userData: UserData option) =
  match userData with
  | Some x ->
    Cmd.ofPromise
      (Fetch.fetchAs<CountBy<UtcDateTimeString> list> "/api/teabags/countby/inserteddate" (Decode.Auto.generateDecoder<CountBy<UtcDateTimeString> list>()) )
      [Fetch.requestHeaders [
          HttpRequestHeaders.Authorization ("Bearer " + x.Token.String)
          HttpRequestHeaders.ContentType "application/json; charset=utf-8"
      ]]
      GetCountByInsertedSuccess
      GetCountByInsertedError
    | _ -> Cmd.none

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
    x = None
  }
  axis =
    match categories with
    | Some x -> Some ( { x = { ``type``= Some "category"; categories = categories; tick = None }; rotated = None })
    | None -> None
}

let toPieChart (data: ResizeArray<PrimitiveArray> option) = Some {
  data = {
    columns = data
    ``type`` = Some ChartType.Pie
    x = None
  }
  axis = None
}

let xData (data: CountBy<Moment> list): PrimitiveArray =
  [| "x" |> U3.Case1 |> Some |]
  |> Array.append <| ((createXAxis data) |> Seq.map (fun x -> x |> U3.Case1 |> Some) |> Array.ofSeq)

let periodMonthData (data: CountBy<Moment> list) =
  [|data |> xData|]
  |> Array.append <| (splitData data)

let toPeriodMonthChart2 (data: CountBy<Moment> list) =
  Some {
    data = {
      x = Some "x"
      columns =   (data |> periodMonthData |> ResizeArray |> Some)
      ``type`` = Some ChartType.Line
    }
    axis = Some ( { x = { ``type``= Some "timeseries"; categories = None; tick = Some({ values = None; format = Some !^"%m-%d" }) }; rotated = None })
  }

let transform (chart: ChartConfig option) (tranformation: ChartTransformation) =
  match chart with
  | Some x ->
    match tranformation with
    | PieToBar -> toBarChart None x.data.columns
    | BarToPie -> toPieChart x.data.columns
  | None -> None

let init (userData: UserData option) =
    let initialModel = {
      countByBrands = None
      countByBagtypes = None
      countByInserted = None
      countBrands = None
      countBagtypes = None
      countInserted = None
      userData = userData
    }
    initialModel, tryAuthorizationRequest getCountByBrandsCmd userData, getCountByBagtypesCmd userData, getCountByInsertedCmd userData

let moment: Fable.Import.Moment.IExports = importAll "moment"
let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetCountByBrandsSuccess data ->
    { model with countByBrands = Some data; countBrands = (mapToData data |> toBarChart (Some (ResizeArray [| "brands" |]))) }, Cmd.none
  | GetCountByBrandsError exn -> model, Cmd.none
  | GetCountByBagtypesSuccess data ->
    { model with countByBagtypes = Some data; countBagtypes = (mapToData data |> toPieChart)}, Cmd.none
  | GetCountByInsertedSuccess data ->
    let dashboardData = data |> List.map(fun x -> {count = x.count; description = moment(U7.Case3 x.description.String)})
    let config =  dashboardData |> toPeriodMonthChart2
    printf "%O" config
    { model with countByInserted = Some dashboardData; countInserted = config }, Cmd.none
  | GetCountByBagtypesError exn -> model, Cmd.none
  | GetCountByInsertedError exn -> model, Cmd.none
  | TransformCountByBrand transformation ->
    {model with countBrands = (transform model.countBrands transformation)}, Cmd.none
  | TransformCountByBagtype transformation ->
    {model with countBagtypes = (transform model.countBagtypes transformation)}, Cmd.none
  | ReloadBrands -> model, tryAuthorizationRequest getCountByBrandsCmd model.userData


