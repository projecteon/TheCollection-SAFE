module Client.Dashboard.State

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fetch

open Fable.Import.Moment.Moment
open Thoth.Json

open Client
open Client.Extensions
open Client.ElmishHelpers
open Client.Dashboard.Types
open Domain.SharedTypes
open Server.Api.Dtos
open Client

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
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/brands" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByBrandsSuccess
    GetCountByBrandsError

let getCountByBagtypesCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/bagtypes" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByBagtypesSuccess
    GetCountByBagtypesError

let getCountByCountryTLDCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/countrytlp" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByCountryTLDSuccess
    GetCountByCountryTLDError

let getCountByInsertedCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<UtcDateTimeString> list> "/api/teabags/countby/inserteddate" (Decode.Auto.generateDecoder<CountBy<UtcDateTimeString> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByInsertedSuccess
    GetCountByInsertedError

let getStatisticsCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<Statistics> "/api/teabags/statistics" (Decode.Auto.generateDecoder<Statistics>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetStatisticsSuccess
    GetStatisticsError

let mapToData (countBy: CountBy<string> list) (count: ReChartHelpers.DataCount) =
  countBy
  |> List.truncate (int count)
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
  |> Array.append <| ((splitData data) |> Array.tail)

let toPeriodMonthChart2 (data: CountBy<Moment> list) =
  Some {
    data = {
      x = Some "x"
      columns =   (data |> periodMonthData |> ResizeArray |> Some)
      ``type`` = Some ChartType.Line
    }
    axis = Some ( { x = { ``type``= Some "timeseries"; categories = None; tick = Some({ values = None; format = Some !^"%B"; culling = false |> U2.Case1 |> Some })}; rotated = None }) // https://github.com/d3/d3-time-format/blob/v2.1.3/README.md#timeFormat
  }

let transform (chart: ChartConfig option) (tranformation: ChartTransformation) =
  match chart with
  | Some x ->
    match tranformation with
    | PieToBar -> toBarChart None x.data.columns
    | BarToPie -> toPieChart x.data.columns
  | None -> None

let dataCountToggle previousCount =
  match previousCount with
  | ReChartHelpers.DataCount.Ten -> ReChartHelpers.DataCount.Twenty
  | ReChartHelpers.DataCount.Twenty -> ReChartHelpers.DataCount.Ten
  | _ -> ReChartHelpers.DataCount.Ten

let mapToHighchatData (data: CountBy<string> list): HighchartData array =
  data
  |> Seq.map (fun x -> {value = x.count; key = x.description})
  |> Seq.toArray

let init =
    let initialModel = {
      statistics = None
      countByBrands = None
      countByBagtypes = None
      countByCountryTLD = None
      countByInserted = None
      countByInsertedHoveredKey = None
      countBrands = None
      countBagtypes = None
      countCountryTLD = None
      countInserted = None
      displayedByBrands = ReChartHelpers.DataCount.Ten
      displayedBrands = ReChartHelpers.DataCount.Ten
    }
    initialModel, getCountByBrandsCmd, getCountByBagtypesCmd, getCountByCountryTLDCmd, getCountByInsertedCmd, getStatisticsCmd

let moment: Fable.Import.Moment.IExports = importAll "moment"
let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetStatisticsSuccess data ->
    { model with statistics = Some data }, Cmd.none
  | GetStatisticsError exn -> model, Cmd.none
  | GetCountByBrandsSuccess data ->
    { model with countByBrands = Some data; countBrands = (mapToData data model.displayedBrands |> toBarChart (Some (ResizeArray [| "brands" |]))) }, Cmd.none
  | GetCountByBrandsError exn -> model, Cmd.none
  | GetCountByBagtypesSuccess data ->
    { model with countByBagtypes = Some data; countBagtypes = (mapToData data ReChartHelpers.DataCount.Ten |> toPieChart)}, Cmd.none
  | GetCountByBagtypesError exn -> model, Cmd.none
  | GetCountByCountryTLDSuccess data ->
    { model with countByCountryTLD = Some data; countCountryTLD = Some (mapToHighchatData data)}, Cmd.none
  | GetCountByCountryTLDError exn -> model, Cmd.none
  | GetCountByInsertedSuccess data ->
    let dashboardData = data |> List.map(fun x -> {count = x.count; description = moment(U7.Case3 x.description.String)})
    let config =  dashboardData |> toPeriodMonthChart2
    { model with countByInserted = Some dashboardData; countInserted = config }, Cmd.none
  | GetCountByInsertedError exn -> model, Cmd.none
  | TransformCountByBrand transformation ->
    {model with countBrands = (transform model.countBrands transformation)}, Cmd.none
  | TransformCountByBagtype transformation ->
    {model with countBagtypes = (transform model.countBagtypes transformation)}, Cmd.none
  | ExpandByBrands -> {model with displayedByBrands = dataCountToggle model.displayedByBrands}, Cmd.none
  | CollapseByBrands -> {model with displayedByBrands = dataCountToggle model.displayedByBrands}, Cmd.none
  | ExpandBrands -> {model with displayedBrands = dataCountToggle model.displayedBrands}, Cmd.ofMsg (GetCountByBrandsSuccess model.countByBrands.Value)
  | CollapseBrands -> {model with displayedBrands = dataCountToggle model.displayedBrands}, Cmd.ofMsg (GetCountByBrandsSuccess model.countByBrands.Value)
  | ToggleCountByInsertedHoveredKey key -> {model with countByInsertedHoveredKey = key}, Cmd.none


